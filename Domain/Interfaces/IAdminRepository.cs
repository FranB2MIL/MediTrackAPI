using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAdminRepository : IRepository<Admin>
    {
        Task<Admin?> GetAdminByEmailAsync(string email);
        Task<bool> AnyAsync();
    }
}