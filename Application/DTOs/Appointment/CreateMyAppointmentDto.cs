using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Appointment;

public class CreateMyAppointmentDto
{
    [Required]
    public int? DoctorId { get; set; }

    [Required]
    public DateTime? Date { get; set; }

    [Required]
    public string StartTime { get; set; } = string.Empty;
}
