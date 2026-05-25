namespace Domain.Entities
{
    public class Consultation
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public Measurement? Measurement { get; set; }
    }
}
