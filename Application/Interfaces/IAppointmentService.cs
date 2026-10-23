using Application.DTOs.Appointment;

namespace Application.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetByRangeAsync(int doctorId, DateTime from, DateTime to);
    Task<AppointmentDto?> GetByIdAsync(int id, int doctorId);
    Task<AppointmentDto?> AddAsync(CreateAppointmentDto dto, int doctorId);
    Task<bool> UpdateAsync(int id, int doctorId, UpdateAppointmentDto dto);
    Task<bool> CancelAsync(int id, int doctorId);
    Task<bool> DeleteAsync(int id, int doctorId);

}