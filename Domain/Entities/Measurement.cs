namespace Domain.Entities
{
    public class Measurement
    {
        public int Id { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double Size { get; set; }
        public double IMC { get; set; }

        public int ConsultationId { get; set; }
        public Consultation Consultation { get; set; } = null!;
    }
}
