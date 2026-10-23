using Application.Common;
using Application.DTOs.Availability;

namespace Application.Interfaces;

public interface IAvailabilityService
{
    Task<IEnumerable<AvailabilityDto>> GetByDoctorIdAsync(int doctorId);
    Task<AvailabilityDto> AddAsync(CreateAvailabilityDto dto, int doctorId);
    Task<UpdateResult> UpdateAsync(int id, int doctorId, UpdateAvailabilityDto dto);
    Task<UpdateResult> DeleteAsync(int id, int doctorId);
}
