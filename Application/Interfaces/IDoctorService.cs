using Application.Common;
using Application.DTOs.Doctor;

namespace Application.Interfaces;

public interface IDoctorService
{
    Task<IEnumerable<DoctorDto>> GetAllAsync();
    Task<DoctorDto?> GetByIdAsync(int id);
    Task<DoctorDto?> AddAsync(CreateDoctorDto createDoctorDto);
    Task<UpdateResult> UpdateAsync(int id, UpdateDoctorDto updateDoctorDto);
    Task<UpdateResult> DeleteAsync(int id);
}