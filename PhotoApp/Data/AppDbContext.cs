using Microsoft.EntityFrameworkCore;
using PhotoApp.Models;

namespace PhotoApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Таблицы пользователей и ролей
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }

        // Структура фотоцентра
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Kiosk> Kiosks { get; set; }

        // Услуги и товары
        public DbSet<Service> Services { get; set; }
        public DbSet<Product> Products { get; set; }

        // Заказы и продажи
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderService> OrderServices { get; set; }
        public DbSet<Sale> Sales { get; set; }

        // Поставки
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplyOrder> SupplyOrders { get; set; }
        public DbSet<SupplyOrderItem> SupplyOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связей для Users -> Roles
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            // Настройка связей для Clients -> Users
            modelBuilder.Entity<Client>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            // Настройка связей для Employees -> Users
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            // Настройка связей для Kiosks -> Branches
            modelBuilder.Entity<Kiosk>()
                .HasOne(k => k.Branch)
                .WithMany(b => b.Kiosks)
                .HasForeignKey(k => k.BranchId);

            // Настройка связей для Orders -> Clients
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Client)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.ClientId);

            // Настройка связей для Orders -> Kiosks
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Kiosk)
                .WithMany(k => k.Orders)
                .HasForeignKey(o => o.KioskId);

            // Настройка связей для Orders -> Branches
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Branch)
                .WithMany(b => b.Orders)
                .HasForeignKey(o => o.BranchId);

            // Настройка связей для OrderServices -> Orders
            modelBuilder.Entity<OrderService>()
                .HasOne(os => os.Order)
                .WithMany(o => o.OrderServices)
                .HasForeignKey(os => os.OrderId);

            // Настройка связей для OrderServices -> Services
            modelBuilder.Entity<OrderService>()
                .HasOne(os => os.Service)
                .WithMany(s => s.OrderServices)
                .HasForeignKey(os => os.ServiceId);

            // Настройка связей для Sales -> Orders
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Order)
                .WithMany(o => o.Sales)
                .HasForeignKey(s => s.OrderId);

            // Настройка связей для Sales -> Products
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Sales)
                .HasForeignKey(s => s.ProductId);

            // Настройка связей для Sales -> Kiosks
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Kiosk)
                .WithMany(k => k.Sales)
                .HasForeignKey(s => s.KioskId);

            // Настройка связей для SupplyOrders -> Suppliers
            modelBuilder.Entity<SupplyOrder>()
                .HasOne(so => so.Supplier)
                .WithMany(s => s.SupplyOrders)
                .HasForeignKey(so => so.SupplierId);

            // Настройка связей для SupplyOrders -> Employees
            modelBuilder.Entity<SupplyOrder>()
                .HasOne(so => so.Employee)
                .WithMany(e => e.SupplyOrders)
                .HasForeignKey(so => so.EmployeeId);

            // Настройка связей для SupplyOrderItems -> SupplyOrders
            modelBuilder.Entity<SupplyOrderItem>()
                .HasOne(soi => soi.SupplyOrder)
                .WithMany(so => so.SupplyOrderItems)
                .HasForeignKey(soi => soi.SupplyOrderId);

            // Настройка связей для SupplyOrderItems -> Products
            modelBuilder.Entity<SupplyOrderItem>()
                .HasOne(soi => soi.Product)
                .WithMany(p => p.SupplyOrderItems)
                .HasForeignKey(soi => soi.ProductId);

            // Настройка индексов для улучшения производительности
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CreatedAt);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Phone)
                .IsUnique();

            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.CreatedAt);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Category);

            // Настройка значений по умолчанию
            modelBuilder.Entity<Order>()
                .Property(o => o.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasDefaultValue("принят");

            modelBuilder.Entity<Order>()
                .Property(o => o.IsPaid)
                .HasDefaultValue(false);

            modelBuilder.Entity<Sale>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<SupplyOrder>()
                .Property(so => so.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<SupplyOrder>()
                .Property(so => so.Status)
                .HasDefaultValue("формируется");

            modelBuilder.Entity<Client>()
                .Property(c => c.IsPro)
                .HasDefaultValue(false);

            modelBuilder.Entity<Client>()
                .Property(c => c.PersonalDiscount)
                .HasDefaultValue(0.00m);

            modelBuilder.Entity<Product>()
                .Property(p => p.StockQuantity)
                .HasDefaultValue(0);

            modelBuilder.Entity<Branch>()
                .Property(b => b.Workplaces)
                .HasDefaultValue(1);

            // Настройка проверочных ограничений
            modelBuilder.Entity<OrderService>()
                .Property(os => os.Quantity)
                .HasAnnotation("CheckConstraint", "Quantity > 0");

            modelBuilder.Entity<Sale>()
                .Property(s => s.Quantity)
                .HasAnnotation("CheckConstraint", "Quantity > 0");

            modelBuilder.Entity<SupplyOrderItem>()
                .Property(soi => soi.Quantity)
                .HasAnnotation("CheckConstraint", "Quantity > 0");
        }
    }
}