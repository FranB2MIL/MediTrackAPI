namespace Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public ICollection<DoctorPatient> DoctorPatients { get; set; } = new List<DoctorPatient>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
