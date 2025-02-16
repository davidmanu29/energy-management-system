using EnergyManagementSystemUser.Controllers.ViewModel;
using EnergyManagementSystemUser.Data;
using EnergyManagementSystemUser.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace EnergyManagementSystemUser.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly EnergyManagementSystemUserDbContext _ctx;
        private readonly IConfiguration _config;

        public AccountController(EnergyManagementSystemUserDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _config = config;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginModel model)
        {
            string type = "";
            Guid id;
            string name;

            if (model.Username == "" || model.Password == "")
            {
                return BadRequest("Fill all the fields");
            }

            List<User> users = _ctx.Users.ToList();

            var registeredUsers = users.Where(user =>
                string.Equals(user.Username, model.Username, StringComparison.CurrentCulture) &&
                string.Equals(user.Password, model.Password)).ToList();

            if (registeredUsers.Any())
            {
                type = registeredUsers.First().IsAdmin ? "Admin" : "User";
                id = registeredUsers.First().Id;
                name = registeredUsers.First().Name;
            }
            else
            {
                return BadRequest("Wrong username or password");
            }


            HttpContext.Session.SetString("Username", model.Username);
            HttpContext.Session.SetString("Type", type);
            HttpContext.Session.SetString("ID", id.ToString());

            var token = CreateToken(new User
            {
                Id = id,
                Name = name,
                IsAdmin = (type == "Admin"),
                Password = model.Password,
                Username = model.Username
            });

            return Ok(new
            {
                token = token,
                username = name,
                roles = new string[] { type }
            });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationModel model)
        {
            const string energyConsumptionDeviceUrl = "http://localhost/devicems/api/user/adduser";

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest("Fill in all the fields");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest("Passwords don't match");
            }

            var existingUser = await _ctx.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

            if (existingUser != null)
            {
                return BadRequest("Username already exists!");
            }

            var newUser = new User { Username = model.Username, Password = model.Password, Name = model.Name };
            _ctx.Users.Add(newUser);
            await _ctx.SaveChangesAsync();

            var userIdForDevices = newUser.Id;
            var json = JsonConvert.SerializeObject(userIdForDevices);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            using var client = new HttpClient(handler);
            client.BaseAddress = new Uri(energyConsumptionDeviceUrl);
            var response = await client.PostAsync("adduser", content);

            Console.WriteLine(response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                return Content("User registered successfully in both microservices.", "text/plain", System.Text.Encoding.UTF8);
            }

            return BadRequest("User registered successfully, but there was an issue adding the user in the devices microservice.");
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Actor, user.IsAdmin ? "Admin" : "User"),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpPost("Logoff")]
        public async Task<IActionResult> LogOff()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return NoContent();
        }
    }
}
