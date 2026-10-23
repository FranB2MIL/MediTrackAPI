using Application.DTOs.Patient;
using Application.Interfaces;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;

[Authorize(Roles = nameof(Role.Patient))]
[ApiController]
[Route("api/me")]
public class PatientPortalController : ControllerBase
{
    private readonly IPatientPortalService _patientPortalService;

    public PatientPortalController(IPatientPortalService patientPortalService)
    {
        _patientPortalService = patientPortalService;
    }

    [HttpGet("consultations")]
    public async Task<IActionResult> GetConsultations([FromQuery] int? doctorId)
    {
        var consultations = await _patientPortalService.GetConsultationsAsync(User.GetUserId(), doctorId);
        return Ok(consultations);
    }

    [HttpGet("doctors")]
    public async Task<IActionResult> GetDoctors()
    {
        var doctors = await _patientPortalService.GetDoctorsAsync(User.GetUserId());
        return Ok(doctors);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await _patientPortalService.GetProfileAsync(User.GetUserId());
        if (profile == null) return NotFound();
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateMyProfileDto dto)
    {
        var profile = await _patientPortalService.UpdateProfileAsync(User.GetUserId(), dto);
        if (profile == null) return NotFound();
        return Ok(profile);
    }
}
