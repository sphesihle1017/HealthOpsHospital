using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Ward
    {
        [Key]
        public int WardId { get; set; }

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Location { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Bed>? Beds { get; set; }


        public ICollection<Admission>? Admissions { get; set; }
    }
}
