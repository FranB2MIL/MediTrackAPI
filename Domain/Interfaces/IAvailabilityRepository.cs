using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IAvailabilityRepository : IRepository<Availability>
    {
        Task<IEnumerable<Availability>> GetByDoctorIdAsync( int doctorId);
        Task<Availability?> GetByIdAndDoctorIdAsync(int id, int doctorId);

    }
}