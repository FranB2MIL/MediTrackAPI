namespace Domain.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public decimal Peso { get; set; }
        public decimal Altura { get; set; }
        public decimal Talla { get; set; }
        public decimal IMC { get; set; }

        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; } = null!;
    }
}
