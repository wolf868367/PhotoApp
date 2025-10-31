using Microsoft.AspNetCore.Mvc;
using PhotoApp.Models;
using Microsoft.EntityFrameworkCore;
using PhotoApp.Services;

namespace PhotoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ApiController
    {
        public ClientsController(DataService dataService, AuthService authService)
            : base(dataService, authService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetClients()
        {
            if (!CheckOperatorAccess()) return Unauthorized();

            var clients = await _dataService.Context.Clients
                .Include(c => c.User)
                .ToListAsync();
            return Ok(clients);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchClients([FromQuery] string phone)
        {
            if (!CheckOperatorAccess()) return Unauthorized();

            var client = await _dataService.Context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Phone.Contains(phone));

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            if (!CheckOperatorAccess()) return Unauthorized();

            _dataService.Context.Clients.Add(client);
            await _dataService.Context.SaveChangesAsync();
            return Ok(client);
        }

        [HttpPut("{id}/pro")]
        public async Task<IActionResult> SetProStatus(int id, [FromBody] bool isPro)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            var client = await _dataService.Context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            client.IsPro = isPro;
            await _dataService.Context.SaveChangesAsync();
            return Ok(client);
        }
    }
}