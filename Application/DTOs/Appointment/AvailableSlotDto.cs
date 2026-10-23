namespace Application.DTOs.Appointment;

public class AvailableSlotDto
{
    public string Date { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int AvailabilityId { get; set; }
}
