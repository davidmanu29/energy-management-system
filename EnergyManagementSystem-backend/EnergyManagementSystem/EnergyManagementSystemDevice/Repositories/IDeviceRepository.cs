using EnergyManagementSystemDevice.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnergyManagementSystemDevice.Repositories
{
    public interface IDeviceRepository
    {
        Task DeleteDevice(Device device);
        bool DeviceExists(Guid deviceId);
        Task<ActionResult<Device?>> GetDeviceById(Guid deviceId);
        Task<IEnumerable<Device?>> GetDeviceByUserId(Guid userId);
        Task<ActionResult<IEnumerable<Device>>> GetDevices();
        Task<ActionResult<IEnumerable<Device>>?> GetUserDevices(Guid userId);
        Task InsertDevice(Device device);
        void UpdateDevice(Device device);
    }
}