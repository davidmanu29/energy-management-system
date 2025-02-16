using EnergyManagementSystemDevice.Models;

namespace EnergyManagementSystemDevice.Repositories
{
    public interface IUserDeviceMappingRepository
    {
        Task InsertUserDeviceMapping(UserDeviceMapping mapping);
        Task<IEnumerable<UserDeviceMapping>> GetMappingsByUserId(Guid userId);
        Task<IEnumerable<UserDeviceMapping>> GetMappingsByDeviceId(Guid deviceId);
        Task DeleteMappingsByUserId(Guid userId);
        Task DeleteMappingsByDeviceId(Guid deviceId);
    }
}
