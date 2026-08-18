using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ConsultationRepository : BaseRepository<Consultation>, IConsultationRepository
{
    public ConsultationRepository(MediTrackDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Consultation>> GetByPatientIdAsync(int patientId)
    {
        return await _dbSet
            .Where(c => c.PatientId == patientId)
            .Include(c => c.Measurement)
            .OrderByDescending(c => c.Date)
            .ToListAsync();
    }

    public async Task<Consultation?> GetByIdWithMeasurementAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Measurement)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}