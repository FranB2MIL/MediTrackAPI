namespace Domain.Entities
{
    public class Consulta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;

        public Medicion? Medicion { get; set; }
    }
}