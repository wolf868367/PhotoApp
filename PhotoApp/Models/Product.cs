using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoApp.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SellingPrice { get; set; }

        public int StockQuantity { get; set; } = 0;

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<SupplyOrderItem> SupplyOrderItems { get; set; } = new List<SupplyOrderItem>();
    }
}
