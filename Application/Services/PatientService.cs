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
        return patients.Select(p => new PatientDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            DateOfBirth = p.DateOfBirth,
            DNI = p.DNI,
            HealthInsurance = p.HealthInsurance
        });
    }

    public async Task<PatientDto?> GetByIdAsync(int id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null) return null;

        return new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            DNI = patient.DNI,
            HealthInsurance = patient.HealthInsurance
        };
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
    public async Task<PatientDto> AddAsync(CreatePatientDto dto, int doctorId)
    {
        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            DNI = dto.DNI,
            HealthInsurance = dto.HealthInsurance
        };
        await _patientRepository.AddAsync(patient);

        var doctorPatient = new DoctorPatient
        {
            DoctorId = doctorId,
            PatientId = patient.Id
        };
        await _patientRepository.AddDoctorPatientAsync(doctorPatient);
        return new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            DNI = patient.DNI,
            HealthInsurance = patient.HealthInsurance
        };
    }

    public async Task UpdateAsync(int id, UpdatePatientDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null) return;

        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.DNI = dto.DNI;
        patient.HealthInsurance = dto.HealthInsurance;

        await _patientRepository.UpdateAsync(patient);
    }

    public async Task DeleteAsync(int id)
    {
        await _patientRepository.DeleteAsync(id);
    }
}
