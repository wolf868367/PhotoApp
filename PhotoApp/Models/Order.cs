
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoApp.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        [Required]
        public int KioskId { get; set; }
        public Kiosk Kiosk { get; set; } = null!;

        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "принят";

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public bool IsPaid { get; set; } = false;

        public string? Notes { get; set; } = null;

        public ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
