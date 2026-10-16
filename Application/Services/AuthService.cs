using Application.DTOs.Auth;
using Application.DTOs.Doctor;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAdminRepository _adminRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDoctorService _doctorService;

    public AuthService(IDoctorRepository doctorRepository, IJwtService jwtService, IPasswordHasher passwordHasher, IDoctorService doctorService, IAdminRepository adminRepository, IPatientRepository patientRepository)
    {
        _doctorRepository = doctorRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _doctorService = doctorService;
        _adminRepository = adminRepository;
        _patientRepository = patientRepository;
    }


    public async Task<LoginResponseDto?> StaffLoginAsync(LoginRequestDto dto)
    {
        IAuthenticatable? user = await _adminRepository.GetAdminByEmailAsync(dto.Email);
        user ??= await _doctorRepository.GetByMailAsync(dto.Email);

        if (user == null || !_passwordHasher.Verify(dto.Password, user.Password))
        {
            return null;
        }
        return BuildResponse(user);
    }

    public async Task<LoginResponseDto?> PatientLoginAsync(LoginRequestDto dto)
    {
        var patient = await _patientRepository.GetByEmailAsync(dto.Email);

        if (patient == null ||
        string.IsNullOrEmpty(patient.Password) ||
        !_passwordHasher.Verify(dto.Password, patient.Password))
        {
            return null;
        }

        return BuildResponse(patient);
    }

    public async Task<LoginResponseDto?> RegisterPatientAsync(RegisterPatientDto registerPatientDto)
    {
        if (await _patientRepository.GetByEmailAsync(registerPatientDto.Email) != null ||
        await _patientRepository.GetByDniAsync(registerPatientDto.DNI) != null)
        {
            return null;
        }

        var patient = new Domain.Entities.Patient
        {
            FirstName = registerPatientDto.FirstName,
            LastName = registerPatientDto.LastName,
            DateOfBirth = registerPatientDto.DateOfBirth!.Value,
            DNI = registerPatientDto.DNI,
            HealthInsurance = registerPatientDto.HealthInsurance,
            InsuranceNumber = registerPatientDto.InsuranceNumber,
            Email = registerPatientDto.Email,
            Password = _passwordHasher.Hash(registerPatientDto.Password),
            PhoneNumber = registerPatientDto.PhoneNumber
        };

        await _patientRepository.AddAsync(patient);

        return BuildResponse(patient);
    }

    private LoginResponseDto BuildResponse(IAuthenticatable user)
    {
        var token = _jwtService.GenerateToken(user);
        return new LoginResponseDto
        {
            Token = token,
            Email = user.Email,
            FirstName = $"{user.FirstName} {user.LastName}",
            Role = user.Role.ToString()
        };
    }
}
