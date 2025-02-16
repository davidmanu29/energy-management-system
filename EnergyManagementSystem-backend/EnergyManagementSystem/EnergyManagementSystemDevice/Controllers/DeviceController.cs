using EnergyManagementSystemDevice.Data;
using EnergyManagementSystemDevice.Models;
using EnergyManagementSystemDevice.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace EnergyManagementSystemDevice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly EnergyManagementSystemDeviceDbContext _ctx;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IUserDeviceMappingRepository _userMappingRepository;

        public DeviceController(EnergyManagementSystemDeviceDbContext ctx, IDeviceRepository deviceRepository,
            IUserDeviceMappingRepository userDeviceMappingRepository)
        {
            _ctx = ctx;
            _deviceRepository = deviceRepository;
            _userMappingRepository = userDeviceMappingRepository;
        }

        //GET: api/Device
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            return await _deviceRepository.GetDevices();
        }

        //GET: api/Device/User/{id}
        [HttpGet("User/{id}")]
        public async Task<ActionResult<IEnumerable<Device>>?> GetUserDevices(Guid id)
        {
            return await _deviceRepository.GetUserDevices(id);
        }

        // GET: api/Device/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(Guid id)
        {
            var device = await _deviceRepository.GetDeviceById(id);

            if (device.Value is null)
            {
                return NotFound();
            }

            return device!;
        }

        // PUT: api/Device/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutDevice([FromRoute]Guid id, [FromBody]UpdateDeviceRequest request)
        {
            var device = new Device
            {
                DeviceId = id,
                MaxConsumptionPerHour = request.MaxConsumptionPerHour,
                Address = request.Address,
                Description = request.Description,
            };

            _deviceRepository.UpdateDevice(device);

            try
            {
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Device>> PostDevice(DeviceDto deviceDto)
        {
            const string userApiUrl = "http://traefik/userms/api/User/";

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
                BaseAddress = new Uri(userApiUrl)
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userResponse = await client.GetAsync($"{deviceDto.UserId}");

            if (!userResponse.IsSuccessStatusCode)
            {
                return BadRequest(new { message = "User does not exist!" });
            }

            var newDevice = new Device
            {
                Address = deviceDto.Address,
                Description = deviceDto.Description!,
                MaxConsumptionPerHour = deviceDto.MaxConsumptionPerHour
            };

            await _deviceRepository.InsertDevice(newDevice);
            await _ctx.SaveChangesAsync();

            var userDeviceMapping = new UserDeviceMapping
            {
                UserId = deviceDto.UserId,
                DeviceId = newDevice.DeviceId
            };

            await _userMappingRepository.InsertUserDeviceMapping(userDeviceMapping);
            await _ctx.SaveChangesAsync();

            return CreatedAtAction("GetDevice", new { id = newDevice.DeviceId }, newDevice);
        }

        // DELETE: api/Device/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDevice(Guid id)
        {
            var device = await _deviceRepository.GetDeviceById(id);

            if (device.Value is null)
            {
                return NotFound();
            }

            await _userMappingRepository.DeleteMappingsByDeviceId(id);

            await _deviceRepository.DeleteDevice(device.Value);
            await _ctx.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Device/DeleteMappingsByUser/{userId}
        [HttpDelete("DeleteMappingsByUser/{userId}")]
        public async Task<IActionResult> DeleteMappingsByUserId(Guid userId)
        {
            await _userMappingRepository.DeleteMappingsByUserId(userId);
            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        private bool DeviceExists(Guid id)
        {
            return _deviceRepository.DeviceExists(id);
        }
    }
}
