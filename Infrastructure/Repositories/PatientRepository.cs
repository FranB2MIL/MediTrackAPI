using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PatientRepository : BaseRepository<Paciente>, IPatientRepository
    {
        public PatientRepository(MediTrackDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Paciente>> GetByMedicIdAsync(int medicId)
        {
            return await _context.MedicoPacientes
                .Where(mp => mp.MedicoId == medicId)
                .Select(mp => mp.Paciente)
                .ToListAsync();
        }
        public async Task<Paciente?> GetByDniAsync(string dni)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.DNI == dni);
        }

        public async Task AddMedicoPacienteAsync(MedicoPaciente medicoPaciente)
        {
            await _context.MedicoPacientes.AddAsync(medicoPaciente);
            await _context.SaveChangesAsync();
        }
    }
}