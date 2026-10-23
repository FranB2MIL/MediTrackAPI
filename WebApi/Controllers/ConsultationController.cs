using Application.DTOs.Consultation;
using Application.Interfaces;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;

[Authorize(Roles = nameof(Role.Doctor))]
[ApiController]
[Route("api/[controller]")]
public class ConsultationController : ControllerBase
{
    private readonly IConsultationService _consultationService;

    public ConsultationController(IConsultationService consultationService)
    {
        _consultationService = consultationService;
    }

    
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatientId(int patientId)
    {
        var doctorId = User.GetDoctorId();
        var consultations = await _consultationService.GetByPatientAndDoctorIdAsync(patientId, doctorId);
        return Ok(consultations);
    }

    

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var doctorId = User.GetDoctorId();
        var consultation = await _consultationService.GetByIdAsync(id, doctorId);
        if (consultation == null) return NotFound();
        return Ok(consultation);
    }

    [HttpPost("patient/{patientId}")]
    public async Task<IActionResult> Create(int patientId, [FromBody] CreateConsultationDto dto)
    {
        var doctorId = User.GetDoctorId();
        var result = await _consultationService.AddAsync(dto, patientId, doctorId);
        if (result == null) return NotFound();

        return Created($"/api/consultation/{result.Id}", result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateConsultationDto dto)
    {
        var doctorId = User.GetDoctorId();
        var response = await _consultationService.UpdateAsync(id, doctorId, dto);
        if (response == null) return NotFound();
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var doctorId = User.GetDoctorId();
        var response = await _consultationService.DeleteAsync(id, doctorId);
        if (!response) return NotFound();
        return NoContent();   
    }
}