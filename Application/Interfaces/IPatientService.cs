using Application.DTOs.Paciente;

namespace Application.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllAsync(int medicoId);
        Task<PatientDto?> GetByIdAsync(int id);
        Task AddAsync(CreatePatientDto patientDto, int medicoId);
        Task UpdateAsync(int id, UpdatePatientDto patientDto);
        Task DeleteAsync(int id);
    }
}