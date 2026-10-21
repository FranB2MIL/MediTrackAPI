using Application.Common;
using Application.DTOs.Patient;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
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
        if (patient == null || string.IsNullOrEmpty(patient.Email)) return UpdateResult.NotFound;

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
        FirstName = patient.FirstName,
        LastName = patient.LastName,
        DateOfBirth = patient.DateOfBirth,
        DNI = patient.DNI,
        HealthInsurance = patient.HealthInsurance
    };

    private static PatientSummaryDto MapToSummaryDto(Patient patient) => new PatientSummaryDto
    {
        Id = patient.Id,
        FirstName = patient.FirstName,
        LastName = patient.LastName,
        DNI = patient.DNI
    };
}
