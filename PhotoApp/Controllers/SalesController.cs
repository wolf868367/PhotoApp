using Microsoft.AspNetCore.Mvc;
using PhotoApp.Models;
using Microsoft.EntityFrameworkCore;
using PhotoApp.Services;

namespace PhotoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ApiController
    {
        public SalesController(DataService dataService, AuthService authService) 
            : base(dataService, authService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetSales()
        {
            if (!CheckOperatorAccess()) return Unauthorized();
            
            var sales = await _dataService.Context.Sales
                .Include(s => s.Product)
                .Include(s => s.Order)
                .Include(s => s.Kiosk)
                .ToListAsync();
            return Ok(sales);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] Sale sale)
        {
            if (!CheckOperatorAccess()) return Unauthorized();
            
            // Проверяем наличие товара
            var product = await _dataService.Context.Products.FindAsync(sale.ProductId);
            if (product == null) return BadRequest("Товар не найден");
            
            if (product.StockQuantity < sale.Quantity)
                return BadRequest("Недостаточно товара на складе");
            
            // Уменьшаем количество товара
            product.StockQuantity -= sale.Quantity;
            
            _dataService.Context.Sales.Add(sale);
            await _dataService.Context.SaveChangesAsync();
            return Ok(sale);
        }
    }
}