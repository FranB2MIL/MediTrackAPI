namespace Domain.Entities
{
    public class MedicoPaciente
    {
        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; } = null!;
    }
}