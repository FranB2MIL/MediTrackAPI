using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(MediTrackDbContext context) : base(context)
    {
        
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorAndRangeAsync(int doctorId, DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(a => a.DoctorId == doctorId && a.Date >= from && a.Date <= to)
            .Include(a => a.Patient)
            .OrderBy(a => a.Date)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdWithPatientAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Appointment?> GetByIdAndDoctorIdAsync(int id, int doctorId)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id && a.DoctorId == doctorId);
    }

    public async Task<bool> TryAddAsync(Appointment appointment)
    {
        await _dbSet.AddAsync(appointment);
        return await TrySaveAsync(appointment);
    }

    public async Task<bool> TryUpdateAsync(Appointment appointment)
    {
        _dbSet.Update(appointment);
        return await TrySaveAsync(appointment);
    }

    // false si se viola el índice único de horario (DoctorId, Date, StartTime) de turnos no cancelados
    private async Task<bool> TrySaveAsync(Appointment appointment)
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteExtendedErrorCode: SqliteUniqueConstraintError })
        {
            _context.Entry(appointment).State = EntityState.Detached;
            return false;
        }
    }

    private const int SqliteUniqueConstraintError = 2067; // SQLITE_CONSTRAINT_UNIQUE

    public async Task<IEnumerable<Appointment>> GetActiveByDoctorAndRangeAsync(int doctorId, DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(a => a.DoctorId == doctorId && a.Date >= from && a.Date <= to && a.Status != AppointmentStatus.Cancelado)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
    {
        return await _dbSet
            .Where(a => a.PatientId == patientId)
            .Include(a => a.Doctor)
            .Include(a => a.Availability)
            .OrderBy(a => a.Date)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAndPatientIdAsync(int id, int patientId)
    {
        return await _dbSet
            .Include(a => a.Doctor)
            .Include(a => a.Availability)
            .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == patientId);
    }

    public async Task<IEnumerable<Appointment>> GetFutureReservedByAvailabilityIdAsync(int availabilityId, DateTime now)
    {
        var today = now.Date;
        var currentTime = TimeOnly.FromDateTime(now);

        return await _dbSet
            .Where(a => a.AvailabilityId == availabilityId &&
                        a.Status == AppointmentStatus.Reservado &&
                        (a.Date > today || (a.Date == today && a.StartTime > currentTime)))
            .ToListAsync();
    }
}