using Application.DTOs.Availability;

namespace Application.Interfaces;

public interface IAvailabilityService
{
    Task<IEnumerable<AvailabilityDto>> GetByDoctorIdAsync(int doctorId);
    Task<AvailabilityDto> AddAsync(CreateAvailabilityDto dto, int doctorId);
    Task<bool> UpdateAsync(int id, int doctorId, UpdateAvailabilityDto dto);
    Task<bool> DeleteAsync(int id, int doctorId);
}
