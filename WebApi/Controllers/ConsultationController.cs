using Application.DTOs.Consultation;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConsultationController : ControllerBase
{
    private readonly IConsultationService _consultationService;

    public ConsultationController(IConsultationService consultationService)
    {
        _consultationService = consultationService;
    }

    [AllowAnonymous]
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatientId(int patientId)
    {
        var consultations = await _consultationService.GetByPatientIdAsync(patientId);
        return Ok(consultations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var consultation = await _consultationService.GetByIdAsync(id);
        if (consultation == null) return NotFound();
        return Ok(consultation);
    }

    [HttpPost("patient/{patientId}")]
    public async Task<IActionResult> Create(int patientId, [FromBody] CreateConsultationDto dto)
    {
        await _consultationService.AddAsync(dto, patientId);
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateConsultationDto dto)
    {
        await _consultationService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _consultationService.DeleteAsync(id);
        return NoContent();
    }
}