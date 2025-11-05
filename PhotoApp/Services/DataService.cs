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
        public async Task AddClientAsync(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
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

        // Методы для поставщиков
        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            return await _context.Suppliers
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int supplierId)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // Методы для поставок
        public async Task AddSupplyOrderAsync(SupplyOrder supplyOrder)
        {
            _context.SupplyOrders.Add(supplyOrder);
            await _context.SaveChangesAsync();
        }

        public async Task AddSupplyOrderItemAsync(SupplyOrderItem item)
        {
            _context.SupplyOrderItems.Add(item);
            await _context.SaveChangesAsync();
        }

        // Метод для обновления количества товаров при получении поставки
        public async Task UpdateProductStockAsync(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity += quantity;
                await _context.SaveChangesAsync();
            }
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
        public async Task<bool> ClientExistsAsync(string phone)
        {
            return await _context.Clients.AnyAsync(c => c.Phone == phone);
        }

        public async Task<Client?> GetClientByPhoneAsync(string phone)
        {
            return await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Phone.Contains(phone));
        }
        public async Task<Client?> GetClientByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Clients
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка поиска клиента: {ex.Message}");
                return null;
            }
        }
        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Role)
                    .OrderBy(u => u.Login)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки пользователей: {ex.Message}");
                return new List<ApplicationUser>();
            }
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            try
            {
                return await _context.Employees
                    .Include(e => e.User)
                    .Include(e => e.Branch)
                    .OrderBy(e => e.FullName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки сотрудников: {ex.Message}");
                return new List<Employee>();
            }
        }

        public async Task AddUserAsync(ApplicationUser user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка добавления пользователя: {ex.Message}");
                throw;
            }
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка добавления сотрудника: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления пользователя: {ex.Message}");
                throw;
            }
        }
        public async Task UpdateClientAsync(Client client)
        {
            try
            {
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления пользователя: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления пользователя: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка удаления пользователя: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Client>> GetClientsWithOrdersAsync()
        {
            try
            {
                return await _context.Clients
                    .Include(c => c.Orders)
                    .OrderBy(c => c.FullName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки клиентов с заказами: {ex.Message}");
                return new List<Client>();
            }
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            try
            {
                return await _context.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки ролей: {ex.Message}");
                return new List<Role>();
            }
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