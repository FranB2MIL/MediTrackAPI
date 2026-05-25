using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IDoctorRepository doctorRepository, IJwtService jwtService)
    {
        _doctorRepository = doctorRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var doctor = await _doctorRepository.GetByMailAsync(loginRequestDto.Email);
        if (doctor == null || doctor.Password != loginRequestDto.Password)
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
}
