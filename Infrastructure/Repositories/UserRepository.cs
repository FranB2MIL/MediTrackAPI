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

    // TPT: EF hace LEFT JOIN a las tablas derivadas y materializa el tipo concreto (Admin, Patient o Doctor).
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }
}
