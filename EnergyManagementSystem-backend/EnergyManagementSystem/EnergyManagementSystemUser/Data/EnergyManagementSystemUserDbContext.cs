using EnergyManagementSystemUser.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyManagementSystemUser.Data
{
    public class EnergyManagementSystemUserDbContext : DbContext
    {
        public EnergyManagementSystemUserDbContext(DbContextOptions<EnergyManagementSystemUserDbContext>
            options ) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.NewGuid(),
                    IsAdmin = true,
                    Name = "Admin",
                    Username = "admin",
                    Password = "admin"
                });
        }

        public DbSet<User> Users { get; set; } 

    }
}
