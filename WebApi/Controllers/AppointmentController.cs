using Application.Common;
using Application.DTOs.Appointment;
using Application.Interfaces;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;

[Authorize(Roles = nameof(Role.Doctor))]
[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    
    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByRange([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var appointments = await _appointmentService.GetByRangeAsync(User.GetDoctorId(), from, to);
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id, User.GetDoctorId());
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
    {
        var (result, created) = await _appointmentService.AddAsync(dto, User.GetDoctorId());
        return result switch
        {
            BookingResult.NotFound => NotFound(),
            BookingResult.Invalid => BadRequest(new { message = "El horario no coincide con la disponibilidad." }),
            BookingResult.Conflict => Conflict(new { message = "Ese horario ya está reservado." }),
            _ => Created($"/api/appointment/{created!.Id}", created)
        };
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto)
    {
        var result = await _appointmentService.UpdateAsync(id, User.GetDoctorId(), dto);
        return result switch
        {
            BookingResult.NotFound => NotFound(),
            BookingResult.Invalid => BadRequest(new { message = "El horario no coincide con la disponibilidad." }),
            BookingResult.Conflict => Conflict(new { message = "Ese horario ya está reservado." }),
            _ => NoContent()
        };
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var ok = await _appointmentService.CancelAsync(id, User.GetDoctorId());
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _appointmentService.DeleteAsync(id, User.GetDoctorId());
        if (!ok) return NotFound();
        return NoContent();
    }
}