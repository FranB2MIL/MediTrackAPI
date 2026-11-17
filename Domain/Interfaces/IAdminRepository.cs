using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAdminRepository : IRepository<Admin>
    {
        
        Task<bool> AnyAsync();
    }
}