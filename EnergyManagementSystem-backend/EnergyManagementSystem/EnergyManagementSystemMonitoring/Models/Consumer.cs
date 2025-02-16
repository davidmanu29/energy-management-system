using EnergyManagementSystemDevice.Models;
using EnergyManagementSystemMonitoring.Data;
using EnergyManagementSystemMonitoring.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Channels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EnergyManagementSystemMonitoring.Models
{
    public class Consumer
    {
        private readonly IDbContextFactory<EnergyManagementSystemMonitoringDbContext> _contextFactory;
        private readonly List<double> _hourlyConsumptions = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<Consumer> _logger;
        private readonly IConfiguration _config;
        private Timer _timer;

        private static bool _ok = false;

        public Consumer(IDbContextFactory<EnergyManagementSystemMonitoringDbContext> contextFactory, IConfiguration configuration, ILogger<Consumer> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _contextFactory = contextFactory;
            _timer = new Timer(ComputeHourlyConsumption!, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            _serviceScopeFactory = serviceScopeFactory;
            _config = configuration;
            _logger = logger;
        }

        public async Task StartConsuming()
        {
            _ = Task.Run(async () =>
            {
                var factory = new ConnectionFactory
                { HostName = "rabbitmq", Port = 5672, UserName = "myuser", Password = "mypassword"};

                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                await channel.QueueDeclareAsync(queue: "ConsumptionData",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                await channel.QueueDeclareAsync(queue: "DeviceAlertsQueue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                await channel.ExchangeDeclareAsync(exchange: "DeviceChangesExchange",
                                     type: ExchangeType.Topic,
                                     durable: true,
                                     autoDelete: false,
                                     arguments: null);

                await channel.QueueBindAsync(queue: "DeviceAlertsQueue",
                    exchange: "DeviceChangesExchange",
                    routingKey: "device.alert");

                _logger.LogInformation(" [*] Waiting for messages.");
                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<EnergyManagementSystemMonitoringDbContext>>();

                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.LogInformation($" [x] Received {message}");
                    var measurement = JsonConvert.DeserializeObject<MeasurementData>(message);

                    using (var context = _contextFactory.CreateDbContext())
                    {
                        if (measurement != null)
                        {
                            context.Measurements.Add(measurement);
                            await context.SaveChangesAsync();
                            _logger.LogInformation("Measurement saved to database.");

                            await ComputeConsumptionAndCheckMax(measurement);
                        }
                    }
                };

                await channel.BasicConsumeAsync(queue: "ConsumptionData",
                    autoAck: true,
                    consumer: consumer);

                _logger.LogInformation("Consumer is running. Press Ctrl+C to exit.");
                await Task.Delay(Timeout.Infinite);
            });
        }

        private async Task ComputeConsumptionAndCheckMax(MeasurementData newMeasurement)
        {
            try
            {
                using (var context = _contextFactory.CreateDbContext())
                {
                    var deviceId = newMeasurement.DeviceId;
                    var currentTime = DateTime.UtcNow;
                    var intervalStart = currentTime.AddMinutes(-5);

                    var deviceMeasurements = await context.Measurements
                        .Where(m => m.DeviceId == deviceId && m.TimeStamp >= intervalStart && m.TimeStamp <= currentTime)
                        .OrderBy(m => m.TimeStamp)
                        .ToListAsync();

                    if (deviceMeasurements.Count >= 2)
                    {
                        var consumption = deviceMeasurements.Last().MeasurementValue - deviceMeasurements.First().MeasurementValue;
                        var maxConsumption = await GetMaxConsumptionPerHourFromDevice(deviceId);

                        if (consumption > maxConsumption)
                        {
                            _logger.LogError($"ALERT: Device {deviceId} exceeded max consumption. Consumption: {consumption}, Max Allowed: {maxConsumption}");

                            var deviceChangeEvent = new DeviceChangeEvent
                            {
                                EventType = "ExceededMaxConsumption",
                                Device = deviceId,
                                Timestamp = DateTime.UtcNow
                            };

                            var deviceChangeJson = JsonConvert.SerializeObject(deviceChangeEvent);
                            var deviceChangeBody = Encoding.UTF8.GetBytes(deviceChangeJson);

                            var factory = new ConnectionFactory
                            { HostName = "rabbitmq", Port = 5672, UserName = "myuser", Password = "mypassword" };

                            await using var connection = await factory.CreateConnectionAsync();
                            await using var channel = await connection.CreateChannelAsync();

                            await channel.BasicPublishAsync(
                                exchange: "DeviceChangesExchange",
                                routingKey: "device.alert",
                                mandatory: false,
                                basicProperties: new BasicProperties(),
                                body: deviceChangeBody.AsMemory(),
                                cancellationToken: CancellationToken.None
                            );

                            _logger.LogInformation($"Published DeviceChangeEvent for Device {deviceId} to 'DeviceChangesExchange' with routing key 'device.alert'.");
                        }

                        var hourlyConsumption = new HourlyConsumption
                        {
                            DeviceId = deviceId,
                            Timestamp = intervalStart,
                            TotalConsumption = consumption
                        };

                        context.HourlyConsumptions.Add(hourlyConsumption);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in ComputeConsumptionAndCheckMax.");
            }
        }

        private async void ComputeHourlyConsumption(object state)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var currentTime = DateTime.UtcNow;
                var intervalStart = currentTime.AddMinutes(-5);

                var measurements = await context.Measurements
                        .Where(m => m.TimeStamp >= intervalStart && m.TimeStamp <= currentTime)
                        .ToListAsync();

                var measurementsByDevice = measurements
                    .GroupBy(m => m.DeviceId)
                    .ToList();

                foreach (var group in measurementsByDevice)
                {
                    var deviceId = group.Key;
                    var deviceMeasurements = group.OrderBy(m => m.TimeStamp).ToList();

                    if (deviceMeasurements.Count >= 6)
                    {
                        var consumption = deviceMeasurements.Last().MeasurementValue - deviceMeasurements.First().MeasurementValue;
                        var maxConsumption = await GetMaxConsumptionPerHourFromDevice(deviceId);

                        if (consumption > maxConsumption)
                        {
                            _logger.LogError($"ALERT: Device {deviceId} exceeded max consumption. Consumption: {consumption}, Max Allowed: {maxConsumption}");
                            _logger.LogWarning($"ALERT: Device {deviceId} exceeded max consumption. Value: {deviceMeasurements.Last().MeasurementValue}");

                            var deviceChangeEvent = new DeviceChangeEvent
                            {
                                EventType = "ExceededMaxConsumption",
                                Device = deviceId,
                                Timestamp = DateTime.UtcNow
                            };

                            var deviceChangeJson = JsonConvert.SerializeObject(deviceChangeEvent);
                            var deviceChangeBody = Encoding.UTF8.GetBytes(deviceChangeJson);

                            var factory = new ConnectionFactory
                            { HostName = "rabbitmq", Port = 5672, UserName = "myuser", Password = "mypassword" };

                            await using var connection = await factory.CreateConnectionAsync();
                            await using var channel = await connection.CreateChannelAsync();

                            await channel.BasicPublishAsync(
                                exchange: "DeviceChangesExchange",
                                routingKey: "device.alert",
                                mandatory: false,
                                basicProperties: new BasicProperties(),
                                body: deviceChangeBody.AsMemory(),
                                cancellationToken: CancellationToken.None
                            );

                            _logger.LogInformation($"Published DeviceChangeEvent for Device {deviceId} to 'DeviceChangesExchange' with routing key 'device.alert'.");
                        }

                        var hourlyConsumption = new HourlyConsumption
                        {
                            DeviceId = deviceId,
                            Timestamp = intervalStart,
                            TotalConsumption = consumption
                        };

                        context.HourlyConsumptions.Add(hourlyConsumption);
                        await context.SaveChangesAsync();

                        _hourlyConsumptions.Add(consumption);
                    }
                }
            }
        }

        private async Task<int> GetMaxConsumptionPerHourFromDevice(Guid deviceId)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            var httpClient = new HttpClient(handler);

            var deviceEndpoint = $"http://traefik/devicems/api/Device/{deviceId}";

            var response = await httpClient.GetAsync(deviceEndpoint);

            if (response.IsSuccessStatusCode)
            {
                var device = await response.Content.ReadFromJsonAsync<Device>();

                if (device != null)
                {
                    return device.MaxConsumptionPerHour;
                }
                else
                {
                    throw new Exception("Device data is null.");
                }
            }
            else
            {
                throw new Exception($"Failed to fetch max consumption from the device. {response.Content.ToString()}");
            }
        }

    }
    public class DeviceChangeEvent
    {
        public string EventType { get; set; }
        public Guid Device { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
