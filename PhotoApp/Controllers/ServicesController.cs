using Microsoft.AspNetCore.Mvc;
using PhotoApp.Models;
using PhotoApp.Services;
using Microsoft.EntityFrameworkCore;

namespace PhotoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ApiController
    {
        public ServicesController(DataService dataService, AuthService authService)
            : base(dataService, authService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            var services = await _dataService.GetServicesAsync();
            return Ok(services);
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] Service service)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            _dataService.Context.Services.Add(service);
            await _dataService.Context.SaveChangesAsync();
            return Ok(service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] Service service)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            var existingService = await _dataService.Context.Services.FindAsync(id);
            if (existingService == null) return NotFound();

            existingService.Name = service.Name;
            existingService.BasePrice = service.BasePrice;
            existingService.Description = service.Description;

            await _dataService.Context.SaveChangesAsync();
            return Ok(existingService);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            var service = await _dataService.Context.Services.FindAsync(id);
            if (service == null) return NotFound();

            _dataService.Context.Services.Remove(service);
            await _dataService.Context.SaveChangesAsync();
            return Ok();
        }
    }
}