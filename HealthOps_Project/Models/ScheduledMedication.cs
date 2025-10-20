using System.ComponentModel.DataAnnotations;

namespace HealthOps_Project.Models
{
    public class ScheduledMedication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MedicationId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Dosage cannot exceed 100 characters.")]
        public string Dosage { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [FutureOrTodayDate(ErrorMessage = "Administered date must be today or a future date.")]
        public DateTime AdministeredDate { get; set; }

        [StringLength(100, ErrorMessage = "Administered By cannot exceed 100 characters.")]
        public string? AdministeredBy { get; set; }

        // Navigation property 
        public virtual Medication? Medication { get; set; }

        [Required]
        public ScheduledMedicationStatus ScheduledMedicationStatus { get; set; } = ScheduledMedicationStatus.Active;
    }

    public enum ScheduledMedicationStatus
    {
        Active,
        Completed,
        Cancelled,
        Deleted
    }

    // Custom validation attribute for future or today's date
    public class FutureOrTodayDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date < DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "The date must be today or a future date.");
                }
            }
            return ValidationResult.Success;
        }
    }
}