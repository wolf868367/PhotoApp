using Microsoft.EntityFrameworkCore;
using PhotoApp.Data;
using PhotoApp.Models;

namespace PhotoApp.Services
{
    public class DataService
    {
        private readonly AppDbContext _context;

        public DataService(AppDbContext context)
        {
            _context = context;
        }

        // Свойство для доступа к контексту из контроллеров
        public AppDbContext Context => _context;

        // Методы для пользователей
        public async Task<ApplicationUser?> GetUserByLoginAsync(string login, string password)
        {
            Console.WriteLine($"Searching user: {login}");

            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);

                Console.WriteLine($"User found: {user != null}");
                if (user != null)
                {
                    Console.WriteLine($"User ID: {user.Id}");
                    Console.WriteLine($"User Login: {user.Login}");
                    Console.WriteLine($"User Role: {user.Role?.Name}");
                }

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserByLoginAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }


        // Методы для заказов
        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Kiosk)
                .Include(o => o.Branch)
                .Include(o => o.OrderServices)
                    .ThenInclude(os => os.Service)
                .Include(o => o.Sales)
                    .ThenInclude(s => s.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Kiosk)
                .Include(o => o.Branch)
                .Include(o => o.OrderServices)
                    .ThenInclude(os => os.Service)
                .Include(o => o.Sales)
                    .ThenInclude(s => s.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        // Метод для поиска клиента по телефону
        public async Task<Client?> GetClientByPhoneAsync(string phone)
        {
            return await _context.Clients
                .FirstOrDefaultAsync(c => c.Phone.Contains(phone));
        }

        // Метод для добавления заказа
        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        // Методы для услуг
        public async Task<List<Service>> GetServicesAsync()
        {
            return await _context.Services
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        // Методы для товаров
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Методы для поставок
        public async Task<List<SupplyOrder>> GetSupplyOrdersAsync()
        {
            return await _context.SupplyOrders
                .Include(so => so.Supplier)
                .Include(so => so.Employee)
                .Include(so => so.SupplyOrderItems)
                    .ThenInclude(soi => soi.Product)
                .OrderByDescending(so => so.CreatedAt)
                .ToListAsync();
        }

        public async Task AddSupplyOrderAsync(SupplyOrder supplyOrder)
        {
            _context.SupplyOrders.Add(supplyOrder);
            await _context.SaveChangesAsync();
        }

        // Методы для отчетов
        public async Task<object> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _context.Orders
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .Include(o => o.OrderServices)
                .Include(o => o.Sales)
                .ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var paidOrders = orders.Count(o => o.IsPaid);

            return new
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                PaidOrders = paidOrders,
                Orders = orders
            };
        }

        // Методы для клиентов
        public async Task<List<Client>> GetClientsAsync()
        {
            return await _context.Clients
                .Include(c => c.User)
                .OrderBy(c => c.FullName)
                .ToListAsync();
        }

        public async Task<Client?> GetClientByPhoneAsync(string phone)
        {
            return await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Phone.Contains(phone));
        }

        // Методы для продаж
        public async Task<List<Sale>> GetSalesAsync()
        {
            return await _context.Sales
                .Include(s => s.Product)
                .Include(s => s.Order)
                .Include(s => s.Kiosk)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        // Метод для расчета стоимости заказа с учетом скидок
        public decimal CalculateOrderTotal(Order order, List<OrderService> orderServices, List<Sale> sales)
        {
            decimal total = 0;

            // Сумма услуг
            foreach (var orderService in orderServices)
            {
                decimal serviceTotal = orderService.Quantity * orderService.UnitPrice;

                // Учет срочности (x2)
                if (orderService.IsUrgent)
                {
                    serviceTotal *= 2;
                }

                total += serviceTotal;
            }

            // Сумма товаров
            total += sales.Sum(s => s.Quantity * s.UnitPrice);

            // Применение скидок клиента
            if (order.Client?.IsPro == true && order.Client.PersonalDiscount > 0)
            {
                total -= total * (order.Client.PersonalDiscount / 100);
            }

            return total;
        }
    }
}