using EnergyManagementSystemDevice.Data;
using EnergyManagementSystemDevice.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyManagementSystemDevice.Repositories
{
    public class UserDeviceMappingRepository : IUserDeviceMappingRepository
    {
        private readonly EnergyManagementSystemDeviceDbContext _ctx;

        public UserDeviceMappingRepository(EnergyManagementSystemDeviceDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task InsertUserDeviceMapping(UserDeviceMapping mapping)
        {
            await _ctx.UserDeviceMappings.AddAsync(mapping);
            await _ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDeviceMapping>> GetMappingsByUserId(Guid userId)
        {
            return await _ctx.UserDeviceMappings
                             .Where(mapping => mapping.UserId == userId)
                             .ToListAsync();
        }

        public async Task<IEnumerable<UserDeviceMapping>> GetMappingsByDeviceId(Guid deviceId)
        {
            return await _ctx.UserDeviceMappings
                             .Where(mapping => mapping.DeviceId == deviceId)
                             .ToListAsync();
        }

        public async Task DeleteMappingsByUserId(Guid userId)
        {
            var mappings = await _ctx.UserDeviceMappings
                                     .Where(mapping => mapping.UserId == userId)
                                     .ToListAsync();

            if (mappings.Any())
            {
                _ctx.UserDeviceMappings.RemoveRange(mappings);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task DeleteMappingsByDeviceId(Guid deviceId)
        {
            var mappings = await _ctx.UserDeviceMappings
                                     .Where(mapping => mapping.DeviceId == deviceId)
                                     .ToListAsync();

            if (mappings.Any())
            {
                _ctx.UserDeviceMappings.RemoveRange(mappings);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}   
