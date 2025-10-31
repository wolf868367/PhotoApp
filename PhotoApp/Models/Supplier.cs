using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Specialization { get; set; }

        public ICollection<SupplyOrder> SupplyOrders { get; set; } = new List<SupplyOrder>();
    }
}
