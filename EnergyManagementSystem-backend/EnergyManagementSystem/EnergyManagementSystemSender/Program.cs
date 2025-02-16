using EnergyManagementSystemSender;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Globalization;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "myuser",
            Password = "mypassword"
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "ConsumptionData",
             durable: true,
             exclusive: false,
             autoDelete: false,
             arguments: null);

        string path = "sensor.csv";
        string deviceIdFilePath = "D:\\Distributed Systems\\a1\\EnergyManagementSystem-backend\\EnergyManagementSystem\\EnergyManagementSystemSender\\deviceId.txt";
        Guid deviceId;

        if (File.Exists(deviceIdFilePath))
        {
            string deviceIdString = File.ReadAllText(deviceIdFilePath).Trim();
            if (!Guid.TryParse(deviceIdString, out deviceId))
            {
                Console.WriteLine($"Invalid GUID in {deviceIdFilePath}. Exiting.");
                return;
            }
        }
        else
        {
            Console.WriteLine($"Device ID file {deviceIdFilePath} not found. Exiting.");
            return;
        }

        if (!File.Exists(path))
        {
            Console.WriteLine($"File {path} not found. Exiting.");
            return;
        }

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            if (!double.TryParse(lines[i], NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                Console.WriteLine($"Invalid value on line {i + 1}. Skipping...");
                continue;
            }

            var data = new MeasurementData(deviceId, DateTime.UtcNow, value);

            var json = JsonConvert.SerializeObject(data);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(
                     exchange: string.Empty,
                     routingKey: "ConsumptionData",
                     mandatory: false,
                     basicProperties: new BasicProperties(),
                     body: body
                );

            Console.WriteLine($" [x] Sent {json}");

            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        Console.WriteLine("Completed sending data.");
    }
}
