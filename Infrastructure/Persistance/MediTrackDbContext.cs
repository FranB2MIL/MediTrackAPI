using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance;

public class MediTrackDbContext : DbContext
{
    public MediTrackDbContext(DbContextOptions<MediTrackDbContext> options) : base(options) { }

    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<DoctorPatient> DoctorPatients { get; set; }
    public DbSet<Consultation> Consultations { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<Study> Studies { get; set; }
    public DbSet<Availability> Availabilities { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Admin> Admins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DoctorPatient>()
            .HasKey(dp => new { dp.DoctorId, dp.PatientId });

        modelBuilder.Entity<Consultation>()
            .HasOne(c => c.Measurement)
            .WithOne(m => m.Consultation)
            .HasForeignKey<Measurement>(m => m.ConsultationId);

        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.Property(m => m.Weight).HasColumnType("REAL");
            entity.Property(m => m.Height).HasColumnType("REAL");
            entity.Property(m => m.Size).HasColumnType("REAL");
            entity.Property(m => m.IMC).HasColumnType("REAL");
        });

        // Un solo turno activo por doctor y horario; los cancelados no cuentan. 2 = AppointmentStatus.Cancelado
        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.DoctorId, a.Date, a.StartTime })
            .IsUnique()
            .HasFilter("\"Status\" <> 2");
    }
}
