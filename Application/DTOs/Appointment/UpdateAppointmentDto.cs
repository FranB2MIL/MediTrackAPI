namespace Application.DTOs.Appointment;

public class UpdateAppointmentDto
{
    public string Date { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? PatientId { get; set; }
}