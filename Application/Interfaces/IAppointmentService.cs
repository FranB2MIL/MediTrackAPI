using Application.DTOs.Appointment;

namespace Application.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetByRangeAsync(int doctorId, DateTime from, DateTime to);
    Task<AppointmentDto?> GetByIdAsync(int id);
    Task<AppointmentDto> AddAsync(CreateAppointmentDto dto, int doctorId);
    Task UpdateAsync(int id, UpdateAppointmentDto dto);
    Task CancelAsync(int id);
    Task DeleteAsync(int id);

}