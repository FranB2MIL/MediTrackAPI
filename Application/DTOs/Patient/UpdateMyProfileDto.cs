using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Patient
{
    public class UpdateMyProfileDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime? DateOfBirth { get; set; }

        public string HealthInsurance { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
