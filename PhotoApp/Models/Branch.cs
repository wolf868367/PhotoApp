using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        public int Workplaces { get; set; } = 1;

        public ICollection<Kiosk> Kiosks { get; set; } = new List<Kiosk>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
