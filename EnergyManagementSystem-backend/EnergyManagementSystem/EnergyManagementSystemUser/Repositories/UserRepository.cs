using EnergyManagementSystemUser.Data;
using EnergyManagementSystemUser.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnergyManagementSystemUser.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EnergyManagementSystemUserDbContext _ctx;

        public UserRepository(EnergyManagementSystemUserDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<ActionResult<IEnumerable<User>>> GetUsersAsync()
        {
            return await _ctx.Users.Where(x => x.IsAdmin == false).ToListAsync();
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _ctx.Users.FirstOrDefaultAsync(user => user.Name == username);
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            return await _ctx.Users.FindAsync(userId);
        }

        public async Task InsertUser(User user)
        {
            await _ctx.Users.AddAsync(user);
        }

        public async Task<User?> UpdateUser(User user)
        {
            var existingUser = await _ctx.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

            if (existingUser == null) return null;

            _ctx.Users.Entry(existingUser).CurrentValues.SetValues(user);

            await _ctx.SaveChangesAsync();

            return user;
        }

        public void DeleteUser(User user)
        {
            _ctx.Users.Remove(user);
        }

        public bool UserExists(Guid userId)
        {
            return (_ctx.Users?.Any(e => e.Id == userId)).GetValueOrDefault();
        }
    }
}
