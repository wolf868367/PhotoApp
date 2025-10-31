using Microsoft.AspNetCore.Mvc;
using PhotoApp.Models;
using Microsoft.EntityFrameworkCore;
using PhotoApp.Services;

namespace PhotoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ApiController
    {
        public ReportsController(DataService dataService, AuthService authService)
            : base(dataService, authService)
        {
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            var report = await _dataService.GetSalesReportAsync(startDate, endDate);
            return Ok(report);
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventoryReport()
        {
            if (!CheckAdminAccess()) return Unauthorized();

            var products = await _dataService.Context.Products.ToListAsync();
            var lowStockProducts = products.Where(p => p.StockQuantity < 10).ToList();

            return Ok(new
            {
                TotalProducts = products.Count,
                LowStockProducts = lowStockProducts,
                TotalInventoryValue = products.Sum(p => p.StockQuantity * p.SellingPrice)
            });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrdersReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            var orders = await _dataService.Context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .Include(o => o.OrderServices)
                .Include(o => o.Sales)
                .ToListAsync();

            var popularServices = orders
                .SelectMany(o => o.OrderServices)
                .GroupBy(os => os.ServiceId)
                .Select(g => new {
                    ServiceId = g.Key,
                    ServiceName = g.First().Service?.Name,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .ToList();

            return Ok(new
            {
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                PopularServices = popularServices
            });
        }
    }
}