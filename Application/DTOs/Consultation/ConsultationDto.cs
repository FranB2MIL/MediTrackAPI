namespace Application.DTOs.Consultation;

public class ConsultationDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PatientId { get; set; }

    public MeasurementDto? Measurement { get; set; }
}