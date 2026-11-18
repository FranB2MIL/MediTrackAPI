using Application.Common;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;

    public AuthService(IJwtService jwtService, IPasswordHasher passwordHasher, IPatientRepository patientRepository, IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _patientRepository = patientRepository;
        _userRepository = userRepository;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var email = EmailNormalizer.Normalize(dto.Email);
        var user = await _userRepository.GetByEmailAsync(email);

        // Misma respuesta si el mail no existe o la contraseña es incorrecta (no revela qué mails están registrados).
        if (user == null || !_passwordHasher.Verify(dto.Password, user.Password))
        {
            return null;
        }
        if (!user.Roles.Any())
        {
            return null;
        }
        return BuildResponse(user);
    }

    public async Task<LoginResponseDto?> RegisterPatientAsync(RegisterPatientDto dto)
    {
        var email = EmailNormalizer.Normalize(dto.Email);

        if (await _userRepository.GetByEmailAsync(email) != null ||
            await _patientRepository.GetByDniAsync(dto.DNI) != null)
        {
            return null;
        }

        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = email,
            Password = _passwordHasher.Hash(dto.Password),
            DateOfBirth = dto.DateOfBirth!.Value,
            DNI = dto.DNI,
            HealthInsurance = dto.HealthInsurance,
            InsuranceNumber = dto.InsuranceNumber,
            PhoneNumber = dto.PhoneNumber
        };

        await _patientRepository.AddAsync(patient);

        return BuildResponse(patient);
    }

    private LoginResponseDto BuildResponse(User user)
    {
        return new LoginResponseDto
        {
            Token = _jwtService.GenerateToken(user),
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Roles = user.Roles.Select(r => r.ToString()).ToList()
        };
    }
}
