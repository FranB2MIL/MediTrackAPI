namespace Domain.Entities
{
    public class Consultation
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public Measurement? Measurement { get; set; }
    }
}
