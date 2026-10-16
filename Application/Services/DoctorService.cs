using Application.Common;
using Application.DTOs.Doctor;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAdminRepository _adminRepository;
    public DoctorService(IDoctorRepository doctorRepository, IPasswordHasher passwordHasher, IAdminRepository adminRepository)
    {
        _doctorRepository = doctorRepository;
        _passwordHasher = passwordHasher;
        _adminRepository = adminRepository;
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

    public async Task<DoctorDto?> AddAsync(CreateDoctorDto createDoctorDto)
    {
        if (await _doctorRepository.GetByMailAsync(createDoctorDto.Email) != null || await _adminRepository.GetAdminByEmailAsync(createDoctorDto.Email) != null)
        {
            return null;
        }
        var doctor = new Doctor
        {
            FirstName = createDoctorDto.FirstName,
            LastName = createDoctorDto.LastName,
            Email = createDoctorDto.Email,
            Password = _passwordHasher.Hash(createDoctorDto.Password)
        };
        await _doctorRepository.AddAsync(doctor);
        return new DoctorDto
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email
        };
    }

    public async Task<UpdateResult> UpdateAsync(int id, UpdateDoctorDto updateDoctorDto)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
    if (doctor == null) return UpdateResult.NotFound;

    var doctorWithEmail = await _doctorRepository.GetByMailAsync(updateDoctorDto.Email);
    var emailUsedByOtherDoctor = doctorWithEmail != null && doctorWithEmail.Id != id;
    var emailUsedByAdmin = await _adminRepository.GetAdminByEmailAsync(updateDoctorDto.Email) != null;

    if (emailUsedByOtherDoctor || emailUsedByAdmin) return UpdateResult.Conflict;

    doctor.FirstName = updateDoctorDto.FirstName;
    doctor.LastName = updateDoctorDto.LastName;
    doctor.Email = updateDoctorDto.Email;

    await _doctorRepository.UpdateAsync(doctor);
    return UpdateResult.Success;
    }

    public async Task DeleteAsync(int id)
    {
        await _doctorRepository.DeleteAsync(id);
    }
}
