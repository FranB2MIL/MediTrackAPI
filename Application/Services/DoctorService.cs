using Application.DTOs.Doctor;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPasswordHasher _passwordHasher;
    public DoctorService(IDoctorRepository doctorRepository, IPasswordHasher passwordHasher)
    {
        _doctorRepository = doctorRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        return doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            FirstName = d.FirstName,
            LastName = d.LastName,
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
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email
        };
    }

    public async Task AddAsync(CreateDoctorDto createDoctorDto)
    {
        var doctor = new Doctor
        {
            FirstName = createDoctorDto.FirstName,
            LastName = createDoctorDto.LastName,
            Email = createDoctorDto.Email,
            Password = _passwordHasher.Hash(createDoctorDto.Password)
        };
        await _doctorRepository.AddAsync(doctor);
    }

    public async Task UpdateAsync(int id, UpdateDoctorDto updateDoctorDto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null) throw new Exception("Doctor not found");

        doctor.FirstName = updateDoctorDto.FirstName;
        doctor.LastName = updateDoctorDto.LastName;
        doctor.Email = updateDoctorDto.Email;

        await _doctorRepository.UpdateAsync(doctor);
    }

    public async Task DeleteAsync(int id)
    {
        await _doctorRepository.DeleteAsync(id);
    }
}
