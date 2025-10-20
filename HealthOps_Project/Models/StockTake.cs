using System;
using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class Stocktake
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ConsumableId { get; set; }

        [Required, StringLength(100)]
        public string WardName { get; set; }

        [Required]
        public int QuantityCounted { get; set; }

        [Required]
        public DateTime DateTaken { get; set; }

        public string Notes { get; set; }

        public virtual Consumable Consumable { get; set; }
    }
}
