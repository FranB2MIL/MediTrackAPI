namespace Application.DTOs.Appointment;

public class AppointmentDto
{
    public int Id { get; set; }
    public string Date { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int AvailabilityId { get; set; }
    public int DoctorId { get; set; }
    public int? PatientId { get; set; }
    public string? PatientName { get; set; }
}