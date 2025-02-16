namespace EnergyManagementSystemDevice.Models
{
    public class UserDeviceMapping
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid DeviceId { get; set; }
    }
}
