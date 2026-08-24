namespace Application.DTOs.Availability;

public class AvailabilityDto
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int DayOfWeek { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int AppointmentDuration { get; set; }
}