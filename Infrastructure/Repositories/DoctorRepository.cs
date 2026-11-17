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



        public async Task<IEnumerable<Doctor>> GetByPatientIdAsync(int patientId)
        {
            return await _dbSet
                .Include(d => d.User)
                .Where(d => d.DoctorPatients.Any(dp => dp.PatientId == patientId))
                .OrderBy(d => d.User.LastName)
                .ThenBy(d => d.User.FirstName)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _dbSet
                .Include(d => d.User)
                .OrderBy(d => d.User.LastName)
                .ThenBy(d => d.User.FirstName)
                .ToListAsync();
        }

        public override async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
