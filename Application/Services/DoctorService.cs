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

    public async Task<CreateDoctorResultDto?> AddAsync(CreateDoctorDto dto)
    {
        var email = EmailNormalizer.Normalize(dto.Email);
        var existingUser = await _userRepository.GetByEmailWithProfilesAsync(email);

        if (existingUser == null)
        {
            if (await _patientRepository.GetByDniAsync(dto.DNI) != null) return null;

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = email,
                Password = _passwordHasher.Hash(dto.Password),
                Doctor = new Doctor(),
                Patient = new Patient
                {
                    DNI = dto.DNI,
                    DateOfBirth = dto.DateOfBirth!.Value,
                    PhoneNumber = dto.PhoneNumber,
                    HealthInsurance = dto.HealthInsurance,
                    InsuranceNumber = dto.InsuranceNumber
                }
            };

            await _userRepository.AddAsync(user);
            return MapToCreateResult(user, linkedToExistingAccount: false);
        }

        // El mail ya pertenece a alguien: solo se vincula si es un paciente que todavía no es médico
        if (existingUser.Doctor != null || existingUser.Admin != null) return null;

        existingUser.Doctor = new Doctor();
        await _userRepository.UpdateAsync(existingUser);
        return MapToCreateResult(existingUser, linkedToExistingAccount: true);
    }

    public async Task<UpdateResult> UpdateAsync(int id, UpdateDoctorDto dto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null) return UpdateResult.NotFound;

        var email = EmailNormalizer.Normalize(dto.Email);
        var userWithEmail = await _userRepository.GetByEmailWithProfilesAsync(email);
        if (userWithEmail != null && userWithEmail.Id != doctor.UserId) return UpdateResult.Conflict;

        doctor.User.FirstName = dto.FirstName;
        doctor.User.LastName = dto.LastName;
        doctor.User.Email = email;

        await _doctorRepository.UpdateAsync(doctor);
        return UpdateResult.Success;
    }

    public async Task DeleteAsync(int id)
    {
        await _doctorRepository.DeleteAsync(id);
    }

    private static DoctorDto MapToDto(Doctor doctor) => new DoctorDto
    {
        Id = doctor.Id,
        FirstName = doctor.User.FirstName,
        LastName = doctor.User.LastName,
        Email = doctor.User.Email
    };

    private static CreateDoctorResultDto MapToCreateResult(User user, bool linkedToExistingAccount) => new CreateDoctorResultDto
    {
        Id = user.Doctor!.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        LinkedToExistingAccount = linkedToExistingAccount
    };
}