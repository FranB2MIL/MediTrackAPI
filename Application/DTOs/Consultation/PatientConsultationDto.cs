namespace Application.DTOs.Consultation;

public class PatientConsultationDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public MeasurementDto? Measurement { get; set; }

    public int DoctorId { get; set; }
    public string DoctorFirstName { get; set; } = string.Empty;
    public string DoctorLastName { get; set; } = string.Empty;
}
