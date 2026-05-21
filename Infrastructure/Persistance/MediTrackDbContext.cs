using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance;

public class MediTrackDbContext : DbContext
{
    public MediTrackDbContext(DbContextOptions<MediTrackDbContext> options) : base(options) { }

    public DbSet<Medico> Medicos { get; set; }
    public DbSet<Paciente> Pacientes { get; set; }
    public DbSet<MedicoPaciente> MedicoPacientes { get; set; }
    public DbSet<Consulta> Consultas { get; set; }
    public DbSet<Medicion> Mediciones { get; set; }
    public DbSet<Estudio> Estudios { get; set; }
    public DbSet<Disponibilidad> Disponibilidades { get; set; }
    public DbSet<Turno> Turnos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicoPaciente>()
            .HasKey(mp => new { mp.MedicoId, mp.PacienteId });
    }
}
