using Domain.Entities.Enums;

namespace Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public AppointmentStatus Status { get; set; }

        public int AvailabilityId { get; set; }
        public Availability Availability { get; set; } = null!;

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public int? PatientId { get; set; }
        public Patient? Patient { get; set; }
    }
}
