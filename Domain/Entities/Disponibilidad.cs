namespace Domain.Entities
{
    public class Disponibilidad
    {
        public int Id { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public int DuracionTurno { get; set; }

        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
