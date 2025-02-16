using EnergyManagementSystemMonitoring.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyManagementSystemMonitoring.Data
{
    public class EnergyManagementSystemMonitoringDbContext : DbContext
    {
        public DbSet<MeasurementData> Measurements { get; set; }

        public DbSet<HourlyConsumption> HourlyConsumptions { get; set; }

        public EnergyManagementSystemMonitoringDbContext(DbContextOptions<EnergyManagementSystemMonitoringDbContext> options) : base(options)
        { 

        }
    }
}
