using Application.DTOs.Appointment;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;

[Authorize]
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
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
    {
        var created = await _appointmentService.AddAsync(dto, User.GetDoctorId());
        return Created($"/api/appointment/{created.Id}", created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto)
    {
        await _appointmentService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        await _appointmentService.CancelAsync(id);
        return NoContent(); 
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _appointmentService.DeleteAsync(id);
        return NoContent();
    }
}