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

    [HttpPost]
    public async Task<IActionResult> Create(CreateConsultationDto dto)
    {
        await _consultationService.AddAsync(dto);
        return Created();
    }
}