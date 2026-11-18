using Application.Common;
using Application.DTOs.Doctor;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IPasswordHasher _passwordHasher;

    public DoctorService(IDoctorRepository doctorRepository, IUserRepository userRepository, IPatientRepository patientRepository, IPasswordHasher passwordHasher)
    {
        _doctorRepository = doctorRepository;
        _userRepository = userRepository;
        _patientRepository = patientRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        return doctors.Select(MapToDto);
    }

    public async Task<DoctorDto?> GetByIdAsync(int id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        return doctor == null ? null : MapToDto(doctor);
    }

    public async Task<DoctorDto?> AddAsync(CreateDoctorDto dto)
    {
        var email = EmailNormalizer.Normalize(dto.Email);

        // EF no permite cambiar el tipo de una entidad (Patient → Doctor), así que un mail ya registrado es conflicto.
        if (await _userRepository.GetByEmailAsync(email) != null) return null;
        if (await _patientRepository.GetByDniAsync(dto.DNI) != null) return null;

        var doctor = new Doctor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = email,
            Password = _passwordHasher.Hash(dto.Password),
            DNI = dto.DNI,
            DateOfBirth = dto.DateOfBirth!.Value,
            PhoneNumber = dto.PhoneNumber,
            HealthInsurance = dto.HealthInsurance,
            InsuranceNumber = dto.InsuranceNumber
        };

        await _doctorRepository.AddAsync(doctor);
        return MapToDto(doctor);
    }

    public async Task<UpdateResult> UpdateAsync(int id, UpdateDoctorDto dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null) return UpdateResult.NotFound;

        var email = EmailNormalizer.Normalize(dto.Email);
        var userWithEmail = await _userRepository.GetByEmailAsync(email);
        if (userWithEmail != null && userWithEmail.Id != doctor.Id) return UpdateResult.Conflict;

        doctor.FirstName = dto.FirstName;
        doctor.LastName = dto.LastName;
        doctor.Email = email;

        await _doctorRepository.UpdateAsync(doctor);
        return UpdateResult.Success;
    }

    // TODO: reemplazar por soft delete en una etapa posterior.
    // Con TPT no se puede quitar solo el rol de médico: borrar el Doctor borra la cuenta entera (Doctors, Patients, Users).
    // Por eso solo se permite si no tiene ningún dato clínico, ni como médico ni como paciente.
    public async Task<UpdateResult> DeleteAsync(int id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null) return UpdateResult.NotFound;

        if (await _doctorRepository.HasClinicalDataAsync(id)) return UpdateResult.Conflict;

        await _doctorRepository.DeleteAsync(id);
        return UpdateResult.Success;
    }

    private static DoctorDto MapToDto(Doctor doctor) => new DoctorDto
    {
        Id = doctor.Id,
        FirstName = doctor.FirstName,
        LastName = doctor.LastName,
        Email = doctor.Email
    };
}
