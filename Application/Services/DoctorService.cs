using Application.DTOs.Doctor;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        return doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            Nombre = d.Nombre,
            Apellido = d.Apellido,
            Email = d.Email
        });
    }
    public async Task<DoctorDto?> GetByIdAsync(int id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null) return null;

        return new DoctorDto
        {
            Id = doctor.Id,
            Nombre = doctor.Nombre,
            Apellido = doctor.Apellido,
            Email = doctor.Email
        };
    }

    public async Task AddAsync(CreateDoctorDto createDoctorDto)
    {
        var doctor = new Doctor
        {
            Nombre = createDoctorDto.Nombre,
            Apellido = createDoctorDto.Apellido,
            Email = createDoctorDto.Email,
            Contraseña = createDoctorDto.Contraseña
        };
        await _doctorRepository.AddAsync(doctor);
    }

    public async Task UpdateAsync(int id, UpdateDoctorDto updateDoctorDto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null) throw new Exception("Doctor no encontrado");

        doctor.Nombre = updateDoctorDto.Nombre;
        doctor.Apellido = updateDoctorDto.Apellido;
        doctor.Email = updateDoctorDto.Email;

        await _doctorRepository.UpdateAsync(doctor);
    }

    public async Task DeleteAsync(int id)
    {
        await _doctorRepository.DeleteAsync(id);
    }
}