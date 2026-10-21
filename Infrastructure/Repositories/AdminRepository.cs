using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AdminRepository : BaseRepository<Admin>, IAdminRepository
{
    public AdminRepository(MediTrackDbContext context) : base(context)
    {
    }

    public async Task<Admin?> GetAdminByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<bool> AnyAsync()
    {
        return await _dbSet.AnyAsync();
    }

    
}