using EnergyManagementSystemDevice.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyManagementSystemDevice.Data
{
    public class EnergyManagementSystemDeviceDbContext : DbContext
    {
        public EnergyManagementSystemDeviceDbContext(DbContextOptions<EnergyManagementSystemDeviceDbContext> options)
            : base(options) { }

        public DbSet<Device> Devices { get; set; }

        public DbSet<UserDeviceMapping> UserDeviceMappings { get; set; }
    }
}
