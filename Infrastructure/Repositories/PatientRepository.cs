using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    // Nota: con TPT, _dbSet (Patients) también devuelve médicos, materializados como Doctor.
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        public PatientRepository(MediTrackDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Patient>> GetByDoctorIdAsync(int doctorId)
        {
            return await _dbSet
                        .Where(p => p.MyDoctors.Any(dp => dp.DoctorId == doctorId))
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
                .Where(p => !p.MyDoctors.Any(dp => dp.DoctorId == doctorId)   // no vinculados todavía
                        && p.Id != doctorId)                                    // y que no sea yo (misma Id en TPT)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
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
                .Include(p => p.MyDoctors)
                .ThenInclude(dp => dp.Doctor)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }
    }
}
