using Domain.Entities.Enums;

namespace Domain.Entities
{
    public class Turno
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public EstadoTurno Estado { get; set; }

        public int DisponibilidadId { get; set; }
        public Disponibilidad Disponibilidad { get; set; } = null!;

        public int MedicoId { get; set; }
        public Medico Medico { get; set; } = null!;

        public int? PacienteId { get; set; }
        public Paciente? Paciente { get; set; }
    }
}