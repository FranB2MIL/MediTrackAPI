
namespace Domain.Entities
{
    public class Doctor 
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<DoctorPatient> DoctorPatients { get; set; } = new List<DoctorPatient>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
