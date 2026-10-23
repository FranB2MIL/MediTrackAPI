using Application.DTOs.Consultation;
using Application.DTOs.Doctor;
using Application.DTOs.Patient;

namespace Application.Interfaces;

public interface IPatientPortalService
{
    Task<IEnumerable<PatientConsultationDto>> GetConsultationsAsync(int patientId, int? doctorId);
    Task<IEnumerable<DoctorSummaryDto>> GetDoctorsAsync(int patientId);
    Task<PatientProfileDto?> GetProfileAsync(int patientId);
    Task<PatientProfileDto?> UpdateProfileAsync(int patientId, UpdateMyProfileDto dto);
}
