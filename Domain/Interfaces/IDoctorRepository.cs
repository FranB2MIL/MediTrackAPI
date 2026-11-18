using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<IEnumerable<Doctor>> GetByPatientIdAsync(int patientId);
        Task<bool> HasClinicalDataAsync(int doctorId);
    }
}
