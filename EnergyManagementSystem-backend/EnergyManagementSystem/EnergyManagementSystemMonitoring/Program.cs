using EnergyManagementSystemMonitoring.Data;
using EnergyManagementSystemMonitoring.Models;
using EnergyManagementSystemMonitoring.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContextFactory<EnergyManagementSystemMonitoringDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MonitorDbConnectionString")), ServiceLifetime.Singleton);

builder.Services.AddSession();
builder.Services.AddMvc();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsPolicyBuilder => corsPolicyBuilder
            .AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod());
});

builder.Services.AddSingleton<Consumer>();
//builder.Services.AddScoped<IMeasurementRepository, MeasurementRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EnergyManagementSystemMonitoringDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

RunConsumer(app);

app.Run();


void RunConsumer(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var consumer = services.GetRequiredService<Consumer>();
            app.Lifetime.ApplicationStarted.Register(async () => {
                await consumer.StartConsuming();
            });
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while starting the consumer.");
        }
    }
}