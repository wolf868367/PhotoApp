using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoApp.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [Phone]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? DiscountCard { get; set; }

        public bool IsPro { get; set; } = false;

        [Column(TypeName = "decimal(5,2)")]
        public decimal PersonalDiscount { get; set; } = 0.00m;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
