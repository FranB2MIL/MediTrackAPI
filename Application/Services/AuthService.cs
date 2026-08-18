using Application.DTOs.Auth;
using Application.DTOs.Doctor;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDoctorService _doctorService;

    public AuthService(IDoctorRepository doctorRepository, IJwtService jwtService, IPasswordHasher passwordHasher, IDoctorService doctorService)
    {
        _doctorRepository = doctorRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _doctorService = doctorService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var doctor = await _doctorRepository.GetByMailAsync(loginRequestDto.Email);
        if (doctor == null || !_passwordHasher.Verify(loginRequestDto.Password, doctor.Password))
        {
            return null;
        }

        var token = _jwtService.GenerateToken(doctor);
        return new LoginResponseDto
        {
            Token = token,
            Email = doctor.Email,
            FirstName = $"{doctor.FirstName} {doctor.LastName}"
        };
    }

    public async Task<LoginResponseDto> RegisterAsync(CreateDoctorDto createDoctorDto)
    {
        await _doctorService.AddAsync(createDoctorDto);
        var doctor = await _doctorRepository.GetByMailAsync(createDoctorDto.Email);
        var token = _jwtService.GenerateToken(doctor!);
        return new LoginResponseDto
        {
            Token = token,
            Email = doctor!.Email,
            FirstName = $"{doctor.FirstName} {doctor.LastName}"
        };
    }
}
