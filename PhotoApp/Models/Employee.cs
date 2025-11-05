// Models/Employee.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotoApp.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        public ICollection<SupplyOrder> SupplyOrders { get; set; } = new List<SupplyOrder>();
    }
}