using System.Globalization;
using Application.Common;
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

    public async Task<(BookingResult Result, AppointmentDto? Appointment)> AddAsync(CreateAppointmentDto dto, int doctorId)
    {
        var availability = await _availabilityRepository.GetByIdAndDoctorIdAsync(dto.AvailabilityId, doctorId);
        if (availability == null) return (BookingResult.NotFound, null);

        var patientId = dto.PatientId!.Value;
        if (!await _patientRepository.IsLinkedAsync(doctorId, patientId))
            return (BookingResult.NotFound, null);

        var date = ParseDate(dto.Date);
        var startTime = ParseTime(dto.StartTime);
        if (!SlotGenerator.Fits(availability, date, startTime)) return (BookingResult.Invalid, null);

        var appointment = new Appointment
        {
            Date = date,
            StartTime = startTime,
            Status = AppointmentStatus.Reservado,
            AvailabilityId = dto.AvailabilityId,
            DoctorId = doctorId,
            PatientId = patientId
        };
        if (!await _appointmentRepository.TryAddAsync(appointment)) return (BookingResult.Conflict, null);

        var created = await _appointmentRepository.GetByIdWithPatientAsync(appointment.Id);
        return (BookingResult.Success, MapToDto(created!));
    }

    public async Task<BookingResult> UpdateAsync(int id, int doctorId, UpdateAppointmentDto dto)
    {
        var appointment = await _appointmentRepository.GetByIdAndDoctorIdAsync(id, doctorId);
        if (appointment == null) return BookingResult.NotFound;

        if (dto.PatientId.HasValue && !await _patientRepository.IsLinkedAsync(doctorId, dto.PatientId.Value))
            return BookingResult.NotFound;

        var date = ParseDate(dto.Date);
        var startTime = ParseTime(dto.StartTime);

        // Solo se valida si cambia el horario, para poder editar el estado de turnos viejos
        // cuya disponibilidad se modificó después.
        if (date != appointment.Date || startTime != appointment.StartTime)
        {
            var availability = await _availabilityRepository.GetByIdAndDoctorIdAsync(appointment.AvailabilityId, doctorId);
            if (availability == null || !SlotGenerator.Fits(availability, date, startTime))
                return BookingResult.Invalid;
        }

        appointment.Date = date;
        appointment.StartTime = startTime;
        appointment.Status = ParseStatus(dto.Status);
        appointment.PatientId = dto.PatientId;

        if (!await _appointmentRepository.TryUpdateAsync(appointment)) return BookingResult.Conflict;
        return BookingResult.Success;
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