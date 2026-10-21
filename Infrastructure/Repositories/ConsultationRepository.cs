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

    public async Task<IEnumerable<Consultation>> GetByPatientAndDoctorIdAsync(int patientId, int doctorId)
    {
        return await _dbSet
            .Where(c => c.PatientId == patientId && c.DoctorId == doctorId)
            .Include(c => c.Measurement)
            .OrderByDescending(c => c.Date)
            .ToListAsync();
    }

    public async Task<Consultation?> GetByIdAndDoctorIdAsync(int id, int doctorId)
    {
        return await _dbSet
            .Include(c => c.Measurement)
            .FirstOrDefaultAsync(c => c.Id == id && c.DoctorId == doctorId);
    }

    public async Task<Consultation?> GetByIdWithMeasurementAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Measurement)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}