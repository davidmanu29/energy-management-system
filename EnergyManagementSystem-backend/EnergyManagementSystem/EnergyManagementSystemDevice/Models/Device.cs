using System.Text.Json.Serialization;

namespace EnergyManagementSystemDevice.Models
{
    public class Device
    {
        public Guid DeviceId { get; set; }
        
        public string Description { get; set; }

        public string? Address { get; set; }

        public int MaxConsumptionPerHour { get; set; }  
    }
}
