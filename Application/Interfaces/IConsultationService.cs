using Application.DTOs.Consultation;

namespace Application.Interfaces;

public interface IConsultationService
{
    Task<IEnumerable<ConsultationDto>> GetByPatientAndDoctorIdAsync(int patientId, int doctorId);
    Task<ConsultationDto?> GetByIdAsync(int id, int doctorId);
    Task<ConsultationDto> AddAsync(CreateConsultationDto dto, int patientId, int doctorId);
    Task<ConsultationDto?> UpdateAsync(int id, int doctorId, UpdateConsultationDto dto);
    Task<bool> DeleteAsync(int id, int doctorId);
}