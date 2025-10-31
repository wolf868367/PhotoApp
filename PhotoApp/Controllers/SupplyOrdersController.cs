using Microsoft.AspNetCore.Mvc;
using PhotoApp.Models;
using PhotoApp.Services;
using Microsoft.EntityFrameworkCore;

namespace PhotoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplyOrdersController : ApiController
    {
        public SupplyOrdersController(DataService dataService, AuthService authService)
            : base(dataService, authService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplyOrders()
        {
            if (!CheckAdminAccess()) return Unauthorized();

            var supplyOrders = await _dataService.GetSupplyOrdersAsync();
            return Ok(supplyOrders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupplyOrder([FromBody] SupplyOrder supplyOrder)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            await _dataService.AddSupplyOrderAsync(supplyOrder);
            return Ok(supplyOrder);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            var supplyOrder = await _dataService.Context.SupplyOrders.FindAsync(id);
            if (supplyOrder == null) return NotFound();

            supplyOrder.Status = status;

            // Если поставка получена, увеличиваем количество товаров
            if (status == "получена")
            {
                var items = await _dataService.Context.SupplyOrderItems
                    .Where(soi => soi.SupplyOrderId == id)
                    .Include(soi => soi.Product)
                    .ToListAsync();

                foreach (var item in items)
                {
                    item.Product.StockQuantity += item.Quantity;
                }
            }

            await _dataService.Context.SaveChangesAsync();
            return Ok(supplyOrder);
        }
    }
}