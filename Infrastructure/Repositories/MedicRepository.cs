using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MedicRepository : BaseRepository<Medico>, IMedicRepository
    {
        public MedicRepository(MediTrackDbContext context) : base(context)
        {
        }

        public async Task<Medico?> GetByMailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(m => m.Email == email);
        }
    }
}