using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoApp.Models
{
    public class SupplyOrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SupplyOrderId { get; set; }
        public SupplyOrder SupplyOrder { get; set; } = null!; // ← добавил = null!

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!; // ← добавил = null!

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}