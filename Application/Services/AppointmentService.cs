using System.Globalization;
using Application.DTOs.Appointment;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    
    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    private static AppointmentDto MapToDto(Appointment appointment) => new AppointmentDto
    {
        Id = appointment.Id,
        Date = appointment.Date.ToString("yyyy-MM-dd"),
        StartTime = appointment.StartTime.ToString("HH:mm"),
        Status = appointment.Status.ToString(),
        AvailabilityId = appointment.AvailabilityId,
        DoctorId = appointment.DoctorId,
        PatientId = appointment.PatientId,
        PatientName = appointment.Patient == null
            ? null
            : $"{appointment.Patient.FirstName} {appointment.Patient.LastName}"
    };

    private static DateTime ParseDate(string value)
    {
        if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        throw new ArgumentException("Date debe tener formato yyyy-MM-dd.");
        return date;
    }

    private static TimeOnly ParseTime(string value)
    {
        if (!TimeOnly.TryParse(value, CultureInfo.InvariantCulture, out var time))
            throw new ArgumentException("StartTime debe tener formato HH:mm.");
        return time;
    }

    private static AppointmentStatus ParseStatus(string value)
    {
        if (!Enum.TryParse<AppointmentStatus>(value, ignoreCase: true, out var status))
            throw new ArgumentException("Status debe ser Disponible, Reservado o Cancelado.");
        return status;
    }

    public async Task<IEnumerable<AppointmentDto>> GetByRangeAsync(int doctorId, DateTime from, DateTime to)
    {
        var appointments = await _appointmentRepository.GetByDoctorAndRangeAsync(doctorId, from, to);
        return appointments.Select(MapToDto);
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdWithPatientAsync(id);
        return appointment == null ? null : MapToDto(appointment);
    }

    public async Task<AppointmentDto> AddAsync(CreateAppointmentDto dto, int doctorId)
    {
        var appointment = new Appointment
        {
            Date = ParseDate(dto.Date),
            StartTime = ParseTime(dto.StartTime),
            Status = dto.PatientId.HasValue
                ? AppointmentStatus.Reservado
                : AppointmentStatus.Disponible,
            AvailabilityId = dto.AvailabilityId,
            DoctorId = doctorId,
            PatientId = dto.PatientId
        };
        await _appointmentRepository.AddAsync(appointment);

        var created = await _appointmentRepository.GetByIdWithPatientAsync(appointment.Id);
        return MapToDto(created!);    
    }

    public async Task UpdateAsync(int id, UpdateAppointmentDto dto)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null) throw new InvalidOperationException("Appointment not found");

        appointment.Date = ParseDate(dto.Date);
        appointment.StartTime = ParseTime(dto.StartTime);
        appointment.Status = ParseStatus(dto.Status);
        appointment.PatientId = dto.PatientId;

        await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task CancelAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null) throw new InvalidOperationException("Appointment not found");

        appointment.Status = AppointmentStatus.Cancelado;
        await _appointmentRepository.UpdateAsync(appointment);
    }

    public async Task DeleteAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null) throw new InvalidOperationException("Appointment not found");

        await _appointmentRepository.DeleteAsync(id);
    }
}