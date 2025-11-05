using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoApp.Models
{
    [Table("order_services")]
    public class OrderService
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!; // ← добавил = null!

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!; // ← добавил = null!

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        public bool IsUrgent { get; set; } = false;
        public string? PaperType { get; set; }
        public string? Format { get; set; }
    }
}