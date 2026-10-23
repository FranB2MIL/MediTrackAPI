using System.Globalization;
using Application.Common;
using Application.DTOs.Availability;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public AvailabilityService(IAvailabilityRepository availabilityRepository, IAppointmentRepository appointmentRepository)
    {
        _availabilityRepository = availabilityRepository;
        _appointmentRepository = appointmentRepository;
    }

    private static AvailabilityDto MapToDto(Availability availability) => new AvailabilityDto
    {
        Id = availability.Id,
        DoctorId = availability.DoctorId,
        DayOfWeek = (int)availability.DayOfWeek,
        StartTime = availability.StartTime.ToString("HH:mm"),
        EndTime = availability.EndTime.ToString("HH:mm"),
        AppointmentDuration = availability.AppointmentDuration
    };

    private static TimeOnly ParseTime(string value, string fieldName)
    {
        if (!TimeOnly.TryParse(value,CultureInfo.InvariantCulture, out var time))
          throw new ArgumentException($"{fieldName} debe tener formato HH:mm");

        return time;
    }

    private static void Validate(int dayOfWeek, TimeOnly start, TimeOnly end, int duration)
    {
        if (dayOfWeek < 0 || dayOfWeek > 6)
            throw new ArgumentException("DayOfWeek debe estar entre 0 (domingo) y 6 (sábado).");

        if (end <= start)
            throw new ArgumentException("EndTime debe ser posterior a StartTime.");

        if (duration <= 0)
            throw new ArgumentException("AppointmentDuration debe ser mayor a cero.");        
    }

    public async Task<IEnumerable<AvailabilityDto>> GetByDoctorIdAsync(int doctorId)
    {
        var availabilities = await _availabilityRepository.GetByDoctorIdAsync(doctorId);
        return availabilities.Select(MapToDto);
    }

    public async Task<AvailabilityDto> AddAsync(CreateAvailabilityDto dto, int doctorId)
    {
        var start = ParseTime(dto.StartTime, "StartTime");
        var end = ParseTime(dto.EndTime, "EndTime");
        Validate(dto.DayOfWeek, start, end, dto.AppointmentDuration);

        var availability = new Availability
        {
            DoctorId = doctorId,
            DayOfWeek = (DayOfWeek)dto.DayOfWeek,
            StartTime = start,
            EndTime = end,
            AppointmentDuration = dto.AppointmentDuration
        };

        await _availabilityRepository.AddAsync(availability);
        return MapToDto(availability);
    }

    public async Task<UpdateResult> UpdateAsync(int id, int doctorId, UpdateAvailabilityDto dto)
    {
        var availability = await _availabilityRepository.GetByIdAndDoctorIdAsync(id, doctorId);
        if (availability == null) return UpdateResult.NotFound;

        var start = ParseTime(dto.StartTime, "StartTime");
        var end = ParseTime(dto.EndTime, "EndTime");
        Validate(dto.DayOfWeek, start, end, dto.AppointmentDuration);

        var updated = new Availability
        {
            DayOfWeek = (DayOfWeek)dto.DayOfWeek,
            StartTime = start,
            EndTime = end,
            AppointmentDuration = dto.AppointmentDuration
        };

        var futureReserved = await _appointmentRepository.GetFutureReservedByAvailabilityIdAsync(id, ArgentinaTime.Now());
        if (futureReserved.Any(a => !SlotGenerator.Fits(updated, a.Date, a.StartTime)))
            return UpdateResult.Conflict;

        availability.DayOfWeek = updated.DayOfWeek;
        availability.StartTime = updated.StartTime;
        availability.EndTime = updated.EndTime;
        availability.AppointmentDuration = updated.AppointmentDuration;

        await _availabilityRepository.UpdateAsync(availability);
        return UpdateResult.Success;
    }

    public async Task<UpdateResult> DeleteAsync(int id, int doctorId)
    {
        var availability = await _availabilityRepository.GetByIdAndDoctorIdAsync(id, doctorId);
        if (availability == null) return UpdateResult.NotFound;

        var futureReserved = await _appointmentRepository.GetFutureReservedByAvailabilityIdAsync(id, ArgentinaTime.Now());
        if (futureReserved.Any()) return UpdateResult.Conflict;

        await _availabilityRepository.DeleteAsync(id);
        return UpdateResult.Success;
    }
}