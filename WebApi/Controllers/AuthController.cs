using Application.DTOs.Auth;
using Application.DTOs.Doctor;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("staff/login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.StaffLoginAsync(dto);
        if (result == null) return Unauthorized(new { message = "Credenciales inválidas" });
        return Ok(result);
    }

    [HttpPost("patient/login")]
    public async Task<IActionResult> PatientLogin([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.PatientLoginAsync(dto);
        if (result == null) return Unauthorized(new { message = "Credenciales inválidas" });
        return Ok(result);
    }

    [HttpPost("patient/register")]
    public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientDto dto)
    {
        var result = await _authService.RegisterPatientAsync(dto);
        if (result == null) return Conflict(new { message = "Ya existe un paciente con el mismo mail o DNI." });
        return Ok(result);
    }
}