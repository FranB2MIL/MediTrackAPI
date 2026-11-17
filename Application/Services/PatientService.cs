using Application.Common;
using Application.DTOs.Patient;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    public PatientService(IPatientRepository patientRepository, IDoctorRepository doctorRepository)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync(int doctorId)
    {
        var patients = await _patientRepository.GetByDoctorIdAsync(doctorId);
        return patients.Select(MapToDto);
    }

    public async Task<IEnumerable<PatientSummaryDto>> GetAvailableAsync(int doctorId)
    {
        var patients = await _patientRepository.GetAvailableForDoctorAsync(doctorId);
        return patients.Select(MapToSummaryDto);
    }

    public async Task<PatientDto?> GetByIdAsync(int id, int doctorId)
    {
        if (!await _patientRepository.IsLinkedAsync(doctorId, id)) return null;

        var patient = await _patientRepository.GetByIdAsync(id);
        return patient == null ? null : MapToDto(patient);
    }

    public async Task<UpdateResult> LinkAsync(int patientId, int doctorId)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null) return UpdateResult.NotFound;

        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null) return UpdateResult.NotFound;

        if (doctor.UserId == patient.UserId) return UpdateResult.Invalid;

        if (await _patientRepository.IsLinkedAsync(doctorId, patientId)) return UpdateResult.Conflict;

        await _patientRepository.AddDoctorPatientAsync(new DoctorPatient
        {
            DoctorId = doctorId,
            PatientId = patientId
        });
        return UpdateResult.Success;
    }

    public async Task<bool> UnlinkAsync(int patientId, int doctorId)
    {
        return await _patientRepository.RemoveDoctorPatientAsync(doctorId, patientId);
    }

    private static PatientDto MapToDto(Patient patient) => new PatientDto
    {
        Id = patient.Id,
        FirstName = patient.User.FirstName,
        LastName = patient.User.LastName,
        DateOfBirth = patient.DateOfBirth,
        DNI = patient.DNI,
        HealthInsurance = patient.HealthInsurance
    };

    private static PatientSummaryDto MapToSummaryDto(Patient patient) => new PatientSummaryDto
    {
        Id = patient.Id,
        FirstName = patient.User.FirstName,
        LastName = patient.User.LastName,
        DNI = patient.DNI
    };
}