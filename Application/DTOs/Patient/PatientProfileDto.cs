namespace Application.DTOs.Patient
{
    public class PatientProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string HealthInsurance { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
