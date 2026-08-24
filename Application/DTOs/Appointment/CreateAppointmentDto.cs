namespace Application.DTOs.Appointment;

public class CreateAppointmentDto
{
    public string Date { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public int AvailabilityId { get; set; }
    public int? PatientId { get; set; }

}