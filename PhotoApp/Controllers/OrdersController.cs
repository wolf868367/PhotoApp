using Microsoft.AspNetCore.Mvc;
using PhotoApp.Models;
using Microsoft.EntityFrameworkCore;
using PhotoApp.Services;

namespace PhotoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ApiController
    {
        public OrdersController(DataService dataService, AuthService authService)
            : base(dataService, authService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            if (!CheckOperatorAccess())
                return Unauthorized();

            var orders = await _dataService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            if (!CheckOperatorAccess())
                return Unauthorized();

            var order = await _dataService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!CheckOperatorAccess())
                return Unauthorized();

            await _dataService.AddOrderAsync(order);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
    }
}