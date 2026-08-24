using Application.DTOs.Availability;

namespace Application.Interfaces;

public interface IAvailabilityService
{
    Task<IEnumerable<AvailabilityDto>> GetByDoctorIdAsync(int doctorId);
    Task<AvailabilityDto> AddAsync(CreateAvailabilityDto dto, int doctorId);
    Task UpdateAsync(int id, UpdateAvailabilityDto dto);
    Task DeleteAsync(int id);
}
