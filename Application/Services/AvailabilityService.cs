using System.Globalization;
using Application.DTOs.Availability;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IAvailabilityRepository _availabilityRepository;

    public AvailabilityService(IAvailabilityRepository availabilityRepository)
    {
        _availabilityRepository = availabilityRepository;
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

    public async Task UpdateAsync(int id, UpdateAvailabilityDto dto)
    {
        var availability = await _availabilityRepository.GetByIdAsync(id);
        if (availability == null) throw new InvalidOperationException("Availability not found.");

        var start = ParseTime(dto.StartTime, "StartTime");
        var end = ParseTime(dto.EndTime, "EndTime");
        Validate(dto.DayOfWeek, start, end, dto.AppointmentDuration);

        availability.DayOfWeek = (DayOfWeek)dto.DayOfWeek;
        availability.StartTime = start;
        availability.EndTime = end;
        availability.AppointmentDuration = dto.AppointmentDuration;

        await _availabilityRepository.UpdateAsync(availability);
    }

    public async Task DeleteAsync(int id)
    {
        var availability = await _availabilityRepository.GetByIdAsync(id);
        if (availability == null) throw new InvalidOperationException("Availability not found.");

        await _availabilityRepository.DeleteAsync(id);
    }
}