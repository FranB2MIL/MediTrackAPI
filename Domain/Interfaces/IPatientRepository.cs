using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IPatientRepository : IRepository<Paciente>
    {
        Task<IEnumerable<Paciente>> GetByMedicIdAsync(int medicId);
        Task<Paciente?> GetByDniAsync(string dni);
    }
}