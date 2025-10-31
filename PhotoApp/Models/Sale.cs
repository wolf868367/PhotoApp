using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoApp.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!; // ← добавил = null!

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!; // ← добавил = null!

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public int KioskId { get; set; }
        public Kiosk Kiosk { get; set; } = null!; // ← добавил = null!
    }
}