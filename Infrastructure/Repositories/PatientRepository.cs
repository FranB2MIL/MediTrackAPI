using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        public PatientRepository(MediTrackDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Patient>> GetByDoctorIdAsync(int doctorId)
        {
            return await _dbSet
                        .Include(p => p.User)
                        .Where(p => p.DoctorPatients.Any(dp => dp.DoctorId == doctorId))
                        .ToListAsync();
        }

        public async Task<Patient?> GetByDniAsync(string dni)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.DNI == dni);
        }


        public async Task AddDoctorPatientAsync(DoctorPatient doctorPatient)
        {
            await _context.DoctorPatients.AddAsync(doctorPatient);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Patient>> GetAvailableForDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(p => p.User)
                .Where(p => !p.DoctorPatients.Any(dp => dp.DoctorId == doctorId)          // no vinculados todavía
                        && (p.User.Doctor == null || p.User.Doctor.Id != doctorId))      // y que no sea yo
                .OrderBy(p => p.User.LastName)
                .ThenBy(p => p.User.FirstName)
                .ToListAsync();
        }

        public async Task<bool> IsLinkedAsync(int doctorId, int patientId)
        {
            return await _context.DoctorPatients
                .AnyAsync(dp => dp.DoctorId == doctorId && dp.PatientId == patientId);
        }

        public async Task<bool> RemoveDoctorPatientAsync(int doctorId, int patientId)
        {
            var deleted = await _context.DoctorPatients
                .Where(dp => dp.DoctorId == doctorId && dp.PatientId == patientId)
                .ExecuteDeleteAsync();
            return deleted > 0;
        }

        public async Task<IEnumerable<Patient>> GetWithDoctorsAsync()
        {
            return await _dbSet
                .Include(p => p.User)
                .Include(p => p.DoctorPatients)
                .ThenInclude(dp => dp.Doctor)
                .ThenInclude(d => d.User)
                .OrderBy(p => p.User.LastName)
                .ThenBy(p => p.User.FirstName)
                .ToListAsync();
        }

        public override async Task<Patient?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
