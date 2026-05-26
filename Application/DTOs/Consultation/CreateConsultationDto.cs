namespace Application.DTOs.Consultation;

public class CreateConsultationDto
{
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PatientId { get; set; }

    public CreateMeasurementDto? Measurement { get; set; }
}