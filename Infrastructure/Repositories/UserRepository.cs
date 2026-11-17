using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(MediTrackDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailWithProfilesAsync(string email)
    {
        return await _dbSet
            .Include(u => u.Doctor)
            .Include(u => u.Patient)
            .Include(u => u.Admin)
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}