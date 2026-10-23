using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(MediTrackDbContext context) : base(context)
        {
        }

        public async Task<Doctor?> GetByMailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Email == email);
        }

        public async Task<IEnumerable<Doctor>> GetByPatientIdAsync(int patientId)
        {
            return await _context.DoctorPatients
                .Where(dp => dp.PatientId == patientId)
                .Select(dp => dp.Doctor)
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToListAsync();
        }
    }
}
