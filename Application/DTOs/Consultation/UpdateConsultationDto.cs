using Application.DTOs.Consultation;

public class UpdateConsultationDto
{
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CreateMeasurementDto? Measurement { get; set; }
}