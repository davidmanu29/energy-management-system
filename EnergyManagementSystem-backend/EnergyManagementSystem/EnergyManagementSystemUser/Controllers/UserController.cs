using EnergyManagementSystemUser.Data;
using EnergyManagementSystemUser.Models;
using EnergyManagementSystemUser.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.WebRequestMethods;

namespace EnergyManagementSystemUser.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly EnergyManagementSystemUserDbContext _ctx;
        private readonly IUserRepository _userRepo;

        public UserController(EnergyManagementSystemUserDbContext ctx, IUserRepository userRepo)
        {
            _ctx = ctx;
            _userRepo = userRepo;
        }

        //GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersAsync()
        {
            return await _userRepo.GetUsersAsync();
        }

        //GET by Id: api/Users/{Id}
        [HttpGet("{Id}")]
        public async Task<ActionResult<User>> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepo.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        //GET by username: api/Users/username/{username}
        [AllowAnonymous]
        [HttpGet("username/{username}")]
        public async Task<ActionResult<User>> GetUser(string username)
        {
            var user = await _userRepo.GetUserByUsername(username);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        //PUT: api/Users/{id}
        [HttpPut("{Id}")]
        public async Task<IActionResult> PutUser(Guid id, UserDto userReq)
        {
            var user = new User
            {
                Id = id,
                Username = userReq.Username,
                Name = userReq.Name,
                Password = userReq.Password,
                IsAdmin = userReq.IsAdmin,
            };

            user = await _userRepo.UpdateUser(user);
            
            if (user == null) return NotFound();

            return Ok();
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            await _userRepo.InsertUser(user);
            await _ctx.SaveChangesAsync();

            return user;
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userRepo.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            // const string deleteMappingsUrl = "https://localhost:7260/api/Device/DeleteMappingsByUser/";
            const string deleteMappingsUrl = "http://traefik/devicems/api/Device/DeleteMappingsByUser/";

            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "No JWT token provided!" });
            }

            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };

            using var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(deleteMappingsUrl)
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var deleteMappingsResponse = await client.DeleteAsync($"{deleteMappingsUrl}{user.Id}");

            if (!deleteMappingsResponse.IsSuccessStatusCode)
                return BadRequest("Failed to delete user-device mappings in the devices microservice.");

            _userRepo.DeleteUser(user);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _userRepo.UserExists(id);
        }
    }
}
