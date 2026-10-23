using Application.Common;
using Application.DTOs.Appointment;
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
    Task<IEnumerable<AvailableSlotDto>?> GetAvailableSlotsAsync(int patientId, int doctorId, DateTime from, DateTime to);
    Task<(BookingResult Result, MyAppointmentDto? Appointment)> BookAsync(int patientId, CreateMyAppointmentDto dto);
    Task<IEnumerable<MyAppointmentDto>> GetMyAppointmentsAsync(int patientId);
    Task<UpdateResult> CancelMyAppointmentAsync(int id, int patientId);
}
