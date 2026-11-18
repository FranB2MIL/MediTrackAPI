using Domain.Entities.Enums;

namespace Domain.Entities
{
    public class Patient : User
    {
        public DateTime DateOfBirth { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string HealthInsurance { get; set; } = string.Empty;
        public string InsuranceNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<Study> Studies { get; set; } = new List<Study>();

        // Lado paciente de las relaciones (vía PatientId). Doctor las hereda, por eso el prefijo "My".
        public ICollection<DoctorPatient> MyDoctors { get; set; } = new List<DoctorPatient>();
        public ICollection<Appointment> MyAppointments { get; set; } = new List<Appointment>();

        public override IEnumerable<Role> Roles => [Role.Patient];
    }
}
