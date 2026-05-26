using Domain.Entities;

namespace Domain.Interfaces;

public interface IConsultationRepository : IRepository<Consultation>
{
    Task<IEnumerable<Consultation>> GetByPatientIdAsync(int patientId);
    Task<Consultation?> GetByIdWithMeasurementAsync(int id);
    
}