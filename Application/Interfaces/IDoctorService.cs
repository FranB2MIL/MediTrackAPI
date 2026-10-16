using Application.Common;
using Application.DTOs.Doctor;
using Domain.Entities;

namespace Application.Interfaces;

public interface IDoctorService
{
    Task<IEnumerable<DoctorDto>> GetAllAsync();
    Task<DoctorDto?> GetByIdAsync(int id);
    Task<DoctorDto?> AddAsync(CreateDoctorDto createDoctorDto);
    Task<UpdateResult> UpdateAsync(int id, UpdateDoctorDto updateDoctorDto);
    Task DeleteAsync(int id);
}