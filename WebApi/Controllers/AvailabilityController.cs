using Application.DTOs.Availability;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Extensions;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly IAvailabilityService _availabilityService;

    public AvailabilityController(IAvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var availabilities = await _availabilityService.GetByDoctorIdAsync(User.GetDoctorId());
        return Ok(availabilities);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAvailabilityDto dto)
    {
        var created = await _availabilityService.AddAsync(dto, User.GetDoctorId());
        return Created($"/api/availability/{created.Id}", created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAvailabilityDto dto)
    {
        await _availabilityService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _availabilityService.DeleteAsync(id);
        return NoContent();
    }
}