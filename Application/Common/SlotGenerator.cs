using Domain.Entities;

namespace Application.Common;

/// <summary>
/// Calcula los turnos de un doctor a partir de sus Availability (mismo algoritmo que slots.js del front).
/// Limitación conocida: no se validan solapamientos entre Availabilities del mismo día. Si dos se solapan
/// con distinta duración, se generan turnos solapados con distinto inicio, y el índice único
/// (DoctorId, Date, StartTime) solo impide turnos con el mismo inicio.
/// </summary>
public static class SlotGenerator
{
    public static IEnumerable<Slot> Generate(IEnumerable<Availability> availabilities, DateTime from, DateTime to)
    {
        var slots = new List<Slot>();

        for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
        {
            foreach (var availability in availabilities.Where(a => a.DayOfWeek == date.DayOfWeek))
            {
                if (availability.AppointmentDuration <= 0) continue;

                var duration = TimeSpan.FromMinutes(availability.AppointmentDuration);
                var end = availability.EndTime.ToTimeSpan();

                for (var start = availability.StartTime.ToTimeSpan(); start + duration <= end; start += duration)
                {
                    slots.Add(new Slot(date, TimeOnly.FromTimeSpan(start), TimeOnly.FromTimeSpan(start + duration), availability.Id));
                }
            }
        }

        return slots.OrderBy(s => s.Date).ThenBy(s => s.StartTime);
    }

    public static bool Fits(Availability availability, DateTime date, TimeOnly start)
    {
        if (availability.AppointmentDuration <= 0) return false;
        if (date.DayOfWeek != availability.DayOfWeek) return false;

        var startSpan = start.ToTimeSpan();
        var availabilityStart = availability.StartTime.ToTimeSpan();
        var duration = TimeSpan.FromMinutes(availability.AppointmentDuration);

        if (startSpan < availabilityStart) return false;
        if (startSpan + duration > availability.EndTime.ToTimeSpan()) return false;

        return (startSpan - availabilityStart).TotalMinutes % availability.AppointmentDuration == 0;
    }

    public static Slot? FindSlot(IEnumerable<Availability> availabilities, DateTime date, TimeOnly start)
    {
        var availability = availabilities.FirstOrDefault(a => Fits(a, date, start));
        if (availability == null) return null;

        return new Slot(date.Date, start, start.AddMinutes(availability.AppointmentDuration), availability.Id);
    }
}
