namespace EnergyManagementSystemSender
{
    internal class MeasurementData
    {
        public Guid DeviceId { get; set; }

        public DateTime TimeStamp { get; set; }

        public double MeasurementValue { get; set; }

        public MeasurementData(Guid deviceId, DateTime timeStamp, double measurementValue)
        {
            DeviceId = deviceId;
            TimeStamp = timeStamp;
            MeasurementValue = measurementValue;
        }
    }
}
