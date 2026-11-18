using Domain.Entities.Enums;

namespace Domain.Entities
{
    // Todo médico es también paciente: hereda los datos y las relaciones de Patient.
    public class Doctor : Patient
    {
        // Lado médico de las relaciones (vía DoctorId).
        public ICollection<DoctorPatient> MyPatients { get; set; } = new List<DoctorPatient>();
        public ICollection<Appointment> Schedule { get; set; } = new List<Appointment>();

        public override IEnumerable<Role> Roles => [Role.Doctor, .. base.Roles];
    }
}
