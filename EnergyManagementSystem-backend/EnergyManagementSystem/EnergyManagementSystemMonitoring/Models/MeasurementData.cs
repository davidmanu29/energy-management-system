namespace EnergyManagementSystemMonitoring.Models
{
    public class MeasurementData
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid DeviceId { get; set; }
        public double MeasurementValue { get; set; }
    }
}
