using Application.Common;
using Application.Interfaces;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize(Roles = nameof(Role.Admin))]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients()
    {
        var patients = await _adminService.GetPatientsAsync();
        return Ok(patients);
    }

    [HttpPost("doctors/{doctorId:int}/patients/{patientId:int}")]
    public async Task<IActionResult> AssignPatient(int doctorId, int patientId)
    {
        var result = await _adminService.AssignPatientAsync(doctorId, patientId);
        return result switch
        {
            UpdateResult.NotFound => NotFound(),
            UpdateResult.Conflict => Conflict(new { message = "El paciente ya está asignado a ese doctor." }),
            _ => NoContent()
        };
    }

    [HttpDelete("doctors/{doctorId:int}/patients/{patientId:int}")]
    public async Task<IActionResult> UnassignPatient(int doctorId, int patientId)
    {
        var removed = await _adminService.UnassignPatientAsync(doctorId, patientId);
        if (!removed) return NotFound();
        return NoContent();
    }
}
