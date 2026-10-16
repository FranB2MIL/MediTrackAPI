using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<IEnumerable<Patient>> GetByDoctorIdAsync(int doctorId);
        Task<Patient?> GetByDniAsync(string dni);
        Task<Patient?> GetByEmailAsync(string email);
        Task AddDoctorPatientAsync(DoctorPatient doctorPatient);
    }
}
