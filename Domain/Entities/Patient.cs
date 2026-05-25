namespace Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string HealthInsurance { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<Study> Studies { get; set; } = new List<Study>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<DoctorPatient> DoctorPatients { get; set; } = new List<DoctorPatient>();

    }
}
