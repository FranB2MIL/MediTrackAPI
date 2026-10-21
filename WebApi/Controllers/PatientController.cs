using Application.Common;
using Application.Interfaces;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;

[Authorize(Roles = nameof(Role.Doctor))]
[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _patientService.GetAllAsync(User.GetDoctorId());
        return Ok(patients);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var patients = await _patientService.GetAvailableAsync(User.GetDoctorId());
        return Ok(patients);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var patient = await _patientService.GetByIdAsync(id, User.GetDoctorId());
        if (patient == null) return NotFound();
        return Ok(patient);
    }

    [HttpPost("{id:int}/link")]
    public async Task<IActionResult> Link(int id)
    {
        var result = await _patientService.LinkAsync(id, User.GetDoctorId());
        return result switch
        {
            UpdateResult.NotFound => NotFound(),
            UpdateResult.Conflict => Conflict(new { message = "El paciente ya está en tu lista." }),
            _ => NoContent()
        };
    }

    [HttpDelete("{id:int}/link")]
    public async Task<IActionResult> Unlink(int id)
    {
        var removed = await _patientService.UnlinkAsync(id, User.GetDoctorId());
        if (!removed) return NotFound();
        return NoContent();
    }
}
