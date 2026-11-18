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
        // TPT: una tabla por tipo (Users, Patients, Doctors, Admins); la PK de cada tabla derivada es FK a su base.
        modelBuilder.Entity<User>(entity =>
        {
            entity.UseTptMappingStrategy();

            // AUTOINCREMENT: el JWT es stateless y todos los roles usan el NameIdentifier como Id. Sin esto, SQLite puede reasignar
            // el Id de la cuenta borrada más alta a un usuario nuevo, y un token vigente de la cuenta borrada accedería a sus datos.
            // Nota: el proveedor SQLite (10.0.11) ignora la configuración por tabla (ToTable(..., t => t.Property(..).UseAutoincrement())),
            // así que se configura en la propiedad y también aparece en Patients/Doctors/Admins, donde no tiene efecto: ahí el Id siempre se inserta explícito (copiado de Users).
            entity.Property(u => u.Id).UseAutoincrement();

            entity.HasIndex(u => u.Email).IsUnique();
            entity.Ignore(u => u.Roles);
        });

        // Doctor hereda las navegaciones de Patient, así que EF no puede inferir los pares inversos: se configuran explícitamente.
        modelBuilder.Entity<DoctorPatient>(entity =>
        {
            entity.HasKey(dp => new { dp.DoctorId, dp.PatientId });

            entity.HasOne(dp => dp.Doctor)
                .WithMany(d => d.MyPatients)
                .HasForeignKey(dp => dp.DoctorId)
                // Restrict: evita el 2º camino de cascada Patients→Doctors→DoctorPatients (el vínculo se borra vía PatientId).
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(dp => dp.Patient)
                .WithMany(p => p.MyDoctors)
                .HasForeignKey(dp => dp.PatientId);
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasOne(a => a.Doctor)
                .WithMany(d => d.Schedule)
                .HasForeignKey(a => a.DoctorId)
                // NoAction: evita el camino doble Doctors→Appointments / Doctors→Availabilities→Appointments; se chequea al final de la sentencia, después de la cascada vía Availability.
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(a => a.Patient)
                .WithMany(p => p.MyAppointments)
                .HasForeignKey(a => a.PatientId)
                // NoAction: SetNull también sería un camino de cascada y chocaría con Patients→Doctors→Availabilities→Appointments.
                .OnDelete(DeleteBehavior.NoAction);

            // Un solo turno activo por doctor y horario; los cancelados no cuentan. 2 = AppointmentStatus.Cancelado
            entity.HasIndex(a => new { a.DoctorId, a.Date, a.StartTime })
                .IsUnique()
                .HasFilter("\"Status\" <> 2");
        });

        modelBuilder.Entity<Consultation>(entity =>
        {
            entity.HasOne(c => c.Patient)
                .WithMany(p => p.Consultations)
                .HasForeignKey(c => c.PatientId);

            entity.HasOne(c => c.Doctor)
                .WithMany()
                .HasForeignKey(c => c.DoctorId)
                // Restrict: evita el 2º camino Patients→Doctors→Consultations y protege la historia clínica de los pacientes si se borra un médico.
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.Measurement)
                .WithOne(m => m.Consultation)
                .HasForeignKey<Measurement>(m => m.ConsultationId);
        });

        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.Property(m => m.Weight).HasColumnType("REAL");
            entity.Property(m => m.Height).HasColumnType("REAL");
            entity.Property(m => m.Size).HasColumnType("REAL");
            entity.Property(m => m.IMC).HasColumnType("REAL");
            // HasColumnType("REAL") y el filtro "\"Status\" <> 2" son sintaxis de SQLite. Si van a Azure SQL (SQL Server), el filtro se escribe [Status] <> 2 y REAL es otro tipo
        });
    }
}
