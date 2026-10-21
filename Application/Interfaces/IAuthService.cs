using Application.DTOs.Auth;
using Application.DTOs.Doctor;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> StaffLoginAsync(LoginRequestDto loginRequestDto);
    Task<LoginResponseDto?> PatientLoginAsync(LoginRequestDto loginRequestDto);
    Task<LoginResponseDto?> RegisterPatientAsync(RegisterPatientDto registerPatientDto);

}