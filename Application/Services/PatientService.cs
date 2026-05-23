using Application.DTOs.Paciente;
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

    public async Task<IEnumerable<PatientDto>> GetAllAsync(int medicoId)
    {
        var patients = await _patientRepository.GetByMedicIdAsync(medicoId);
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

    public async Task AddAsync(CreatePatientDto dto, int medicoId)
    {
        var patient = new Paciente
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            FechaDeNacimiento = dto.FechaNacimiento,
            DNI = dto.DNI,
            ObraSocial = dto.ObraSocial
        };
        await _patientRepository.AddAsync(patient);

        var medicoPaciente = new MedicoPaciente
        {
            MedicoId = medicoId,
            PacienteId = patient.Id
        };
        await _patientRepository.AddMedicoPacienteAsync(medicoPaciente);
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