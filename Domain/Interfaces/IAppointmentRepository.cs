using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByDoctorAndRangeAsync(int doctorId, DateTime from, DateTime to);
        Task<Appointment?> GetByIdWithPatientAsync(int id);
        Task<Appointment?> GetByIdAndDoctorIdAsync(int id, int doctorId);
        Task<bool> TryAddAsync(Appointment appointment);
        Task<bool> TryUpdateAsync(Appointment appointment);
        Task<IEnumerable<Appointment>> GetActiveByDoctorAndRangeAsync(int doctorId, DateTime from, DateTime to);
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<Appointment?> GetByIdAndPatientIdAsync(int id, int patientId);
        Task<IEnumerable<Appointment>> GetFutureReservedByAvailabilityIdAsync(int availabilityId, DateTime now);
    }
}