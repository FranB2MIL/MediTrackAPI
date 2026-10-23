using Application.Common;
using Application.DTOs.Appointment;

namespace Application.Interfaces;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetByRangeAsync(int doctorId, DateTime from, DateTime to);
    Task<AppointmentDto?> GetByIdAsync(int id, int doctorId);
    Task<(BookingResult Result, AppointmentDto? Appointment)> AddAsync(CreateAppointmentDto dto, int doctorId);
    Task<BookingResult> UpdateAsync(int id, int doctorId, UpdateAppointmentDto dto);
    Task<bool> CancelAsync(int id, int doctorId);
    Task<bool> DeleteAsync(int id, int doctorId);

}