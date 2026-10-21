using Application.Common;
using Application.DTOs.Patient;

namespace Application.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllAsync(int doctorId);
        Task<IEnumerable<PatientSummaryDto>> GetAvailableAsync(int doctorId);
        Task<PatientDto?> GetByIdAsync(int id, int doctorId);
        Task<UpdateResult> LinkAsync(int patientId, int doctorId);
        Task<bool> UnlinkAsync(int patientId, int doctorId);
    }
}
