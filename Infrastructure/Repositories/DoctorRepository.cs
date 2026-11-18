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
                .Where(d => d.MyPatients.Any(dp => dp.PatientId == patientId))
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _dbSet
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToListAsync();
        }

        // Datos clínicos en cualquiera de los dos roles: como médico (de terceros) o como paciente (propios).
        public async Task<bool> HasClinicalDataAsync(int doctorId)
        {
            // Como médico
            if (await _context.DoctorPatients.AnyAsync(dp => dp.DoctorId == doctorId)) return true;
            if (await _context.Consultations.AnyAsync(c => c.DoctorId == doctorId)) return true;
            if (await _context.Appointments.AnyAsync(a => a.DoctorId == doctorId && a.PatientId != null)) return true;

            // Como paciente
            if (await _context.DoctorPatients.AnyAsync(dp => dp.PatientId == doctorId)) return true;
            if (await _context.Consultations.AnyAsync(c => c.PatientId == doctorId)) return true;
            if (await _context.Studies.AnyAsync(s => s.PatientId == doctorId)) return true;
            return await _context.Appointments.AnyAsync(a => a.PatientId == doctorId);
        }
    }
}
