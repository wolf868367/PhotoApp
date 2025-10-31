using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoApp.Models
{
    public class SupplyOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!; // ← добавил = null!

        [Required]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!; // ← добавил = null!

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "формируется";

        [Column(TypeName = "decimal(10,2)")]
        public decimal? TotalAmount { get; set; }

        public ICollection<SupplyOrderItem> SupplyOrderItems { get; set; } = new List<SupplyOrderItem>();
    }
}