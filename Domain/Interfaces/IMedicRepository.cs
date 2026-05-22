using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMedicRepository : IRepository<Medico>
    {
        Task<Medico?> GetByMailAsync(string mail);
    }
}