using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<IEnumerable<Patient>> GetByDoctorIdAsync(int doctorId);
        Task<Patient?> GetByDniAsync(string dni);
        Task<Patient?> GetByEmailAsync(string email);
        Task AddDoctorPatientAsync(DoctorPatient doctorPatient);
        Task<IEnumerable<Patient>> GetAvailableForDoctorAsync(int doctorId);
        Task<bool> IsLinkedAsync(int doctorId, int patientId);
        Task<bool> RemoveDoctorPatientAsync(int doctorId, int patientId);
        Task<IEnumerable<Patient>> GetWithDoctorsAsync();
    }
}
