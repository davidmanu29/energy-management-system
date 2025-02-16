using EnergyManagementSystemUser.Models;
using Microsoft.AspNetCore.Mvc;

namespace EnergyManagementSystemUser.Repositories
{
    public interface IUserRepository
    {
        void DeleteUser(User user);
        Task<User?> UpdateUser(User user);
        Task<User?> GetUserById(Guid userId);
        Task<User?> GetUserByUsername(string username);
        Task<ActionResult<IEnumerable<User>>> GetUsersAsync();
        Task InsertUser(User user);
        bool UserExists(Guid userId);
    }
}