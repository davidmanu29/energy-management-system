namespace EnergyManagementSystemDevice.Models
{
    public class UpdateDeviceRequest
    {
        public string Description { get; set; }

        public string? Address { get; set; }

        public int MaxConsumptionPerHour { get; set; }
    }
}
