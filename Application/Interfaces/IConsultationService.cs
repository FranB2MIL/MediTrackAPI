using Application.DTOs.Consultation;

namespace Application.Interfaces;

public interface IConsultationService
{
    Task<IEnumerable<ConsultationDto>> GetByPatientIdAsync(int patientId);
    Task<ConsultationDto?> GetByIdAsync(int id);
    Task AddAsync(CreateConsultationDto dto);
    Task UpdateAsync(int id, UpdateConsultationDto dto);
    Task DeleteAsync(int id);
}