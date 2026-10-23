using Application.DTOs.Consultation;
using Application.DTOs.Doctor;
using Application.DTOs.Patient;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class PatientPortalService : IPatientPortalService
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;

    public PatientPortalService(IConsultationRepository consultationRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository)
    {
        _consultationRepository = consultationRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
    }

    public async Task<IEnumerable<PatientConsultationDto>> GetConsultationsAsync(int patientId, int? doctorId)
    {
        var consultations = await _consultationRepository.GetByPatientIdAsync(patientId, doctorId);
        return consultations.Select(MapToConsultationDto);
    }

    public async Task<IEnumerable<DoctorSummaryDto>> GetDoctorsAsync(int patientId)
    {
        var doctors = await _doctorRepository.GetByPatientIdAsync(patientId);
        return doctors.Select(MapToDoctorSummaryDto);
    }

    public async Task<PatientProfileDto?> GetProfileAsync(int patientId)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId);
        return patient == null ? null : MapToProfileDto(patient);
    }

    public async Task<PatientProfileDto?> UpdateProfileAsync(int patientId, UpdateMyProfileDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null) return null;

        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.DateOfBirth = dto.DateOfBirth!.Value;
        patient.HealthInsurance = dto.HealthInsurance;
        patient.InsuranceNumber = dto.InsuranceNumber;
        patient.PhoneNumber = dto.PhoneNumber;

        await _patientRepository.UpdateAsync(patient);
        return MapToProfileDto(patient);
    }

    private static PatientConsultationDto MapToConsultationDto(Consultation consultation) => new PatientConsultationDto
    {
        Id = consultation.Id,
        Date = consultation.Date,
        Reason = consultation.Reason,
        Description = consultation.Description,
        Measurement = consultation.Measurement == null ? null : new MeasurementDto
        {
            Id = consultation.Measurement.Id,
            Weight = consultation.Measurement.Weight,
            Height = consultation.Measurement.Height,
            Size = consultation.Measurement.Size,
            IMC = consultation.Measurement.IMC
        },
        DoctorId = consultation.DoctorId,
        DoctorFirstName = consultation.Doctor.FirstName,
        DoctorLastName = consultation.Doctor.LastName
    };

    private static DoctorSummaryDto MapToDoctorSummaryDto(Doctor doctor) => new DoctorSummaryDto
    {
        Id = doctor.Id,
        FirstName = doctor.FirstName,
        LastName = doctor.LastName
    };

    private static PatientProfileDto MapToProfileDto(Patient patient) => new PatientProfileDto
    {
        Id = patient.Id,
        FirstName = patient.FirstName,
        LastName = patient.LastName,
        DateOfBirth = patient.DateOfBirth,
        DNI = patient.DNI,
        HealthInsurance = patient.HealthInsurance,
        InsuranceNumber = patient.InsuranceNumber,
        PhoneNumber = patient.PhoneNumber,
        Email = patient.Email
    };
}
