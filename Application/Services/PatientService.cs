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
            Nombre = p.Nombre,
            Apellido = p.Apellido,
            FechaNacimiento = p.FechaDeNacimiento,
            DNI = p.DNI,
            ObraSocial = p.ObraSocial
        });
    }

    public async Task<PatientDto?> GetByIdAsync(int id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null) return null;

        return new PatientDto
        {
            Id = patient.Id,
            Nombre = patient.Nombre,
            Apellido = patient.Apellido,
            FechaNacimiento = patient.FechaDeNacimiento,
            DNI = patient.DNI,
            ObraSocial = patient.ObraSocial
        };
    }

    public async Task AddAsync(CreatePatientDto dto, int doctorId)
    {
        var patient = new Patient
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            FechaDeNacimiento = dto.FechaNacimiento,
            DNI = dto.DNI,
            ObraSocial = dto.ObraSocial
        };
        await _patientRepository.AddAsync(patient);

        var doctorPatient = new DoctorPatient
        {
            DoctorId = doctorId,
            PatientId = patient.Id
        };
        await _patientRepository.AddDoctorPatientAsync(doctorPatient);
    }

    public async Task UpdateAsync(int id, UpdatePatientDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null) return;

        patient.Nombre = dto.Nombre;
        patient.Apellido = dto.Apellido;
        patient.FechaDeNacimiento = dto.FechaNacimiento;
        patient.DNI = dto.DNI;
        patient.ObraSocial = dto.ObraSocial;

        await _patientRepository.UpdateAsync(patient);
    }

    public async Task DeleteAsync(int id)
    {
        await _patientRepository.DeleteAsync(id);
    }
}
