using Microsoft.AspNetCore.Mvc;
using PhotoApp.Models;
using Microsoft.EntityFrameworkCore;
using PhotoApp.Services;

namespace PhotoApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ApiController
    {
        public ProductsController(DataService dataService, AuthService authService)
            : base(dataService, authService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _dataService.GetProductsAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            _dataService.Context.Products.Add(product);
            await _dataService.Context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (!CheckAdminAccess()) return Unauthorized();

            var existingProduct = await _dataService.Context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound();

            existingProduct.Name = product.Name;
            existingProduct.Category = product.Category;
            existingProduct.SellingPrice = product.SellingPrice;
            existingProduct.StockQuantity = product.StockQuantity;

            await _dataService.Context.SaveChangesAsync();
            return Ok(existingProduct);
        }
    }
}