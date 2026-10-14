using Domain.Entities;

namespace Domain.Interfaces;

public interface IConsultationRepository : IRepository<Consultation>
{
    Task<IEnumerable<Consultation>> GetByPatientAndDoctorIdAsync(int patientId, int doctorId);
    Task<Consultation?> GetByIdAndDoctorIdAsync(int id, int doctorId);
    Task<Consultation?> GetByIdWithMeasurementAsync(int id);
    
}