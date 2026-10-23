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
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IPatientRepository _patientRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository, IAvailabilityRepository availabilityRepository, IPatientRepository patientRepository)
    {
        _appointmentRepository = appointmentRepository;
        _availabilityRepository = availabilityRepository;
        _patientRepository = patientRepository;
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

    public async Task<AppointmentDto?> GetByIdAsync(int id, int doctorId)
    {
        var appointment = await _appointmentRepository.GetByIdAndDoctorIdAsync(id, doctorId);
        return appointment == null ? null : MapToDto(appointment);
    }

    public async Task<AppointmentDto?> AddAsync(CreateAppointmentDto dto, int doctorId)
    {
        var availability = await _availabilityRepository.GetByIdAndDoctorIdAsync(dto.AvailabilityId, doctorId);
        if (availability == null) return null;

        if (dto.PatientId.HasValue && !await _patientRepository.IsLinkedAsync(doctorId, dto.PatientId.Value))
            return null;

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

    public async Task<bool> UpdateAsync(int id, int doctorId, UpdateAppointmentDto dto)
    {
        var appointment = await _appointmentRepository.GetByIdAndDoctorIdAsync(id, doctorId);
        if (appointment == null) return false;

        if (dto.PatientId.HasValue && !await _patientRepository.IsLinkedAsync(doctorId, dto.PatientId.Value))
            return false;

        appointment.Date = ParseDate(dto.Date);
        appointment.StartTime = ParseTime(dto.StartTime);
        appointment.Status = ParseStatus(dto.Status);
        appointment.PatientId = dto.PatientId;

        await _appointmentRepository.UpdateAsync(appointment);
        return true;
    }

    public async Task<bool> CancelAsync(int id, int doctorId)
    {
        var appointment = await _appointmentRepository.GetByIdAndDoctorIdAsync(id, doctorId);
        if (appointment == null) return false;

        appointment.Status = AppointmentStatus.Cancelado;
        await _appointmentRepository.UpdateAsync(appointment);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int doctorId)
    {
        var appointment = await _appointmentRepository.GetByIdAndDoctorIdAsync(id, doctorId);
        if (appointment == null) return false;

        await _appointmentRepository.DeleteAsync(appointment.Id);
        return true;
    }
}