namespace Domain.Entities
{
    public class Medico
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;

        public ICollection<MedicoPaciente> MedicoPacientes { get; set; } = new List<MedicoPaciente>();
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}