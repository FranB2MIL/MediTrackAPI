namespace Domain.Entities
{
    public class Paciente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaDeNacimiento { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string ObraSocial { get; set; } = string.Empty;
        public string NumeroAfiliado { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string NumeroDeTelefono { get; set; } = string.Empty;

        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
        public ICollection<Estudio> Estudios { get; set; } = new List<Estudio>();
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
        public ICollection<MedicoPaciente> MedicoPacientes { get; set; } = new List<MedicoPaciente>();

    }
}