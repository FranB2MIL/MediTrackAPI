using Application.Common;
using Application.DTOs.Doctor;
using Application.DTOs.Patient;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class AdminService : IAdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;

    public AdminService(IDoctorRepository doctorRepository, IPatientRepository patientRepository)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
    }

    public async Task<IEnumerable<AdminPatientDto>> GetPatientsAsync()
    {
        var patients = await _patientRepository.GetWithDoctorsAsync();
        return patients.Select(MapToDto);
    }

    public async Task<UpdateResult> AssignPatientAsync(int doctorId, int patientId)
    {
        var doctor = await _doctorRepository.GetByIdAsync(doctorId);
        if (doctor == null) return UpdateResult.NotFound;

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

    public async Task<bool> UnassignPatientAsync(int doctorId, int patientId)
    {
        return await _patientRepository.RemoveDoctorPatientAsync(doctorId, patientId);
    }

    private static AdminPatientDto MapToDto(Patient patient) => new AdminPatientDto
    {
        Id = patient.Id,
        FirstName = patient.FirstName,
        LastName = patient.LastName,
        DNI = patient.DNI,
        DateOfBirth = patient.DateOfBirth,
        Email = patient.Email,
        PhoneNumber = patient.PhoneNumber,
        HealthInsurance = patient.HealthInsurance,
        InsuranceNumber = patient.InsuranceNumber,
        Doctors = patient.DoctorPatients
            .Select(dp => dp.Doctor)
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .Select(MapToDoctorSummaryDto)
            .ToList()
    };

    private static DoctorSummaryDto MapToDoctorSummaryDto(Doctor doctor) => new DoctorSummaryDto
    {
        Id = doctor.Id,
        FirstName = doctor.FirstName,
        LastName = doctor.LastName
    };
}
