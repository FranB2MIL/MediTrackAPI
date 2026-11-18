using Application.Common;
using Application.DTOs.Doctor;
using Application.Interfaces;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Authorize(Roles = nameof(Role.Admin))]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAll()
        {
            var doctors = await _doctorService.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetById(int id)
        {
            var doctor = await _doctorService.GetByIdAsync(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }


        [HttpPost]
        public async Task<ActionResult<DoctorDto>> Create([FromBody] CreateDoctorDto dto)
        {
            var result = await _doctorService.AddAsync(dto);
            if (result == null) return Conflict(new { message = "Ya existe una cuenta con ese mail o un paciente con ese DNI." });
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorDto dto)
        {
            var result = await _doctorService.UpdateAsync(id, dto);
            return result switch
            {
                UpdateResult.NotFound => NotFound(),
                UpdateResult.Conflict => Conflict(new { message = "Ya existe una cuenta con ese mail." }),
                _ => NoContent()
            };
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _doctorService.DeleteAsync(id);
            return result switch
            {
                UpdateResult.NotFound => NotFound(),
                UpdateResult.Conflict => Conflict(new { message = "El médico tiene datos clínicos asociados (como médico o como paciente) y no se puede eliminar." }),
                _ => NoContent()
            };
        }
    }
}