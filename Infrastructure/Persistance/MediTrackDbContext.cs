using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance;

public class MediTrackDbContext : DbContext
{
    public MediTrackDbContext(DbContextOptions<MediTrackDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
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
        modelBuilder.Entity<User>(entity =>
    {
        entity.HasIndex(u => u.Email).IsUnique();
        entity.Ignore(u => u.Roles);

        entity.HasOne(u => u.Doctor)
            .WithOne(d => d.User)
            .HasForeignKey<Doctor>(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(u => u.Patient)
            .WithOne(p => p.User)
            .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(u => u.Admin)
            .WithOne(a => a.User)
            .HasForeignKey<Admin>(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    });
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
            // HasColumnType("REAL") y el filtro "\"Status\" <> 2" son sintaxis de SQLite. Si van a Azure SQL (SQL Server), el filtro se escribe [Status] <> 2 y REAL es otro tipo
        });

        // Un solo turno activo por doctor y horario; los cancelados no cuentan. 2 = AppointmentStatus.Cancelado
        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.DoctorId, a.Date, a.StartTime })
            .IsUnique()
            .HasFilter("\"Status\" <> 2");
    }
}
