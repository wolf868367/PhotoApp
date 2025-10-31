using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Models
{
    public class Kiosk
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Phone { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
