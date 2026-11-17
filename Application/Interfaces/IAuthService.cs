using Application.DTOs.Auth;
using Application.DTOs.Doctor;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
    
    Task<LoginResponseDto?> RegisterPatientAsync(RegisterPatientDto registerPatientDto);

}