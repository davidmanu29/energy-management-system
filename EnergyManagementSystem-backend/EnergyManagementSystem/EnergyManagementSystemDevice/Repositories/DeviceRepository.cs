using EnergyManagementSystemDevice.Data;
using EnergyManagementSystemDevice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnergyManagementSystemDevice.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly EnergyManagementSystemDeviceDbContext _ctx;

        public DeviceRepository(EnergyManagementSystemDeviceDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            return await _ctx.Devices.ToListAsync();
        }

        public async Task<ActionResult<IEnumerable<Device>>?> GetUserDevices(Guid userId)
        {
            var deviceIds = await _ctx.UserDeviceMappings
                                      .Where(mapping => mapping.UserId == userId)
                                      .Select(mapping => mapping.DeviceId)
                                      .ToListAsync();

            if (!deviceIds.Any())
            {
                return null;
            }

            var devices = await _ctx.Devices
                                    .Where(device => deviceIds.Contains(device.DeviceId))
                                    .ToListAsync();

            return devices;
        }


        public async Task<ActionResult<Device?>> GetDeviceById(Guid deviceId)
        {
            var device = await _ctx.Devices.FindAsync(deviceId);

            return device;
        }

        public async Task<IEnumerable<Device?>> GetDeviceByUserId(Guid userId)
        {
            var deviceIds = await _ctx.UserDeviceMappings
                                      .Where(mapping => mapping.UserId == userId)
                                      .Select(mapping => mapping.DeviceId)
                                      .ToListAsync();

            var devices = await _ctx.Devices
                                    .Where(device => deviceIds.Contains(device.DeviceId))
                                    .ToListAsync();

            return devices;
        }


        public async Task InsertDevice(Device device)
        {
            await _ctx.Devices.AddAsync(device);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteDevice(Device device)
        {
            _ctx.Devices.Remove(device);
            await _ctx.SaveChangesAsync();
        }

        public void UpdateDevice(Device device)
        {
            _ctx.Entry(device).State = EntityState.Modified;
            _ctx.SaveChanges();
        }

        public bool DeviceExists(Guid deviceId)
        {
            return (_ctx.Devices?.Any(device => device.DeviceId == deviceId)).GetValueOrDefault();
        }
    }
}
