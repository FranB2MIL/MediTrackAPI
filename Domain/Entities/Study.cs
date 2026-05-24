namespace Domain.Entities
{
    public class Study
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        // public DateTime Fecha { get; set; }
        public string ArchivoUrl { get; set; } = string.Empty;

        public int PatientId { get; set; }
        public Patient Patient { get; set; } = null!;
    }
}
