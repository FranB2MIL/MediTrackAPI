using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AvailabilityRepository : BaseRepository<Availability>, IAvailabilityRepository
{
    public AvailabilityRepository(MediTrackDbContext context) : base(context)
    {
        
    }

    public async Task<IEnumerable<Availability>> GetByDoctorIdAsync(int doctorId)
    {
        return await _dbSet
            .Where(a => a.DoctorId == doctorId)
            .OrderByDescending(a=> a.DayOfWeek)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }
}