using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByDoctorAndRangeAsync(int doctorId, DateTime from, DateTime to);
        Task<Appointment?> GetByIdWithPatientAsync(int id);
    }
}