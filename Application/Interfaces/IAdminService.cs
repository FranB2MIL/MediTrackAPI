using Application.Common;
using Application.DTOs.Patient;

namespace Application.Interfaces;

public interface IAdminService
{
    Task<IEnumerable<AdminPatientDto>> GetPatientsAsync();
    Task<UpdateResult> AssignPatientAsync(int doctorId, int patientId);
    Task<bool> UnassignPatientAsync(int doctorId, int patientId);
}
