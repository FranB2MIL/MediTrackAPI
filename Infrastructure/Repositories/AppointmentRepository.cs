using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
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
}