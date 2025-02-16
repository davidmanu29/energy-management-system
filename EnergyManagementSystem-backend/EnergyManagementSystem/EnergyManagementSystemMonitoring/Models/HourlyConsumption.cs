namespace EnergyManagementSystemMonitoring.Models
{
    public class HourlyConsumption
    {
        public Guid Id { get; set; } 
        public Guid DeviceId { get; set; }
        public DateTime Timestamp { get; set; } 
        public double TotalConsumption { get; set; }
    }
}
