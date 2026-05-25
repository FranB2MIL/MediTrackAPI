namespace Domain.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal Size { get; set; }
        public decimal IMC { get; set; }

        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; } = null!;
    }
}
