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
            return await _context.DoctorPatients
                .Where(dp => dp.DoctorId == doctorId)
                .Select(dp => dp.Patient)
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
    }
}
