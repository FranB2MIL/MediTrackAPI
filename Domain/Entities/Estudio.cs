namespace Domain.Entities
{
    public class Estudio
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        // public DateTime Fecha { get; set; }
        public string ArchivoUrl { get; set; } = string.Empty;

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;
    }
}