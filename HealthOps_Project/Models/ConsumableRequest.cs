using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class ConsumableRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ConsumableId { get; set; }

        [Required, StringLength(100)]
        public string WardName { get; set; }

        [Required]
        public int QuantityRequested { get; set; }

        [Required, StringLength(50)]
        public string Status { get; set; }

        public DateTime RequestedAt { get; set; }

        public DateTime? ReceivedAt { get; set; }

        [ForeignKey("ConsumableId")]
        public virtual Consumable Consumable { get; set; }
    }
}
