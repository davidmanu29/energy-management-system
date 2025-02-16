using System.ComponentModel.DataAnnotations;

namespace EnergyManagementSystemDevice.Models
{
    public class DeviceDto
    {
        [Required]
        public string? Address { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public int MaxConsumptionPerHour { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}
