using Application.DTOs.Patient;

namespace Application.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllAsync(int doctorId);
        Task<PatientDto?> GetByIdAsync(int id);
        Task <PatientDto> AddAsync(CreatePatientDto patientDto, int doctorId);
        Task UpdateAsync(int id, UpdatePatientDto patientDto);
        Task DeleteAsync(int id);
    }
}
