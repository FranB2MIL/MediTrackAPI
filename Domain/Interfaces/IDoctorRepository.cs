using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<Doctor?> GetByMailAsync(string mail);
        Task<IEnumerable<Doctor>> GetByPatientIdAsync(int patientId);
    }
}
