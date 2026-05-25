namespace Application.DTOs.Patient
{
    public class UpdatePatientDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string HealthInsurance { get; set; } = string.Empty;
    }
}
