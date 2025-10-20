using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthOps_Project.Models
{
    public class WardStock
    {
        public int Id { get; set; }

        [Required]
        public int ConsumableId { get; set; }

        [Required, StringLength(100)]
        public string WardName { get; set; }

        [Required]
        public int QuantityOnHand { get; set; }

        [ForeignKey("ConsumableId")]
        public virtual Consumable Consumable { get; set; }
    }
}
