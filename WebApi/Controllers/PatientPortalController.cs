using Application.Common;
using Application.DTOs.Appointment;
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

    [HttpGet("doctors/{doctorId:int}/available-slots")]
    public async Task<IActionResult> GetAvailableSlots(int doctorId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        if (to.Date < from.Date || (to.Date - from.Date).TotalDays > MaxSlotRangeDays)
            return BadRequest(new { message = $"El rango debe tener 'to' >= 'from' y no superar {MaxSlotRangeDays} días." });

        var slots = await _patientPortalService.GetAvailableSlotsAsync(User.GetUserId(), doctorId, from, to);
        if (slots == null) return NotFound();
        return Ok(slots);
    }

    [HttpPost("appointments")]
    public async Task<IActionResult> Book([FromBody] CreateMyAppointmentDto dto)
    {
        var (result, appointment) = await _patientPortalService.BookAsync(User.GetUserId(), dto);
        return result switch
        {
            BookingResult.NotFound => NotFound(),
            BookingResult.Invalid => BadRequest(new { message = "El horario está en el pasado o fuera de la disponibilidad del doctor." }),
            BookingResult.Conflict => Conflict(new { message = "Ese horario ya fue reservado." }),
            _ => Created($"/api/me/appointments/{appointment!.Id}", appointment)
        };
    }

    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointments()
    {
        var appointments = await _patientPortalService.GetMyAppointmentsAsync(User.GetUserId());
        return Ok(appointments);
    }

    [HttpPut("appointments/{id:int}/cancel")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        var result = await _patientPortalService.CancelMyAppointmentAsync(id, User.GetUserId());
        return result switch
        {
            UpdateResult.NotFound => NotFound(),
            UpdateResult.Conflict => Conflict(new { message = "El turno ya pasó o ya está cancelado." }),
            _ => NoContent()
        };
    }

    private const int MaxSlotRangeDays = 31;
}
