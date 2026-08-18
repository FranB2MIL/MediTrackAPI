using Application.DTOs.Consultation;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ConsultationService : IConsultationService
{
    private readonly IConsultationRepository _consultationRepository;

    public ConsultationService(IConsultationRepository consultationRepository)
    {
        _consultationRepository = consultationRepository;
    }

    public async Task<IEnumerable<ConsultationDto>> GetByPatientIdAsync(int patientId)
    {
        var consultations = await _consultationRepository.GetByPatientIdAsync(patientId);
        return consultations.Select(c => new ConsultationDto
        {
            Id = c.Id,
            Date = c.Date,
            Reason = c.Reason,
            Description = c.Description,
            PatientId = c.PatientId
        });
    }

    private static double CalculateImc(double weight, double height)
    {
        if (height <= 0) throw new ArgumentException("Height must be greater than zero.");
        return Math.Round(weight / (height * height), 2);
    }

    public async Task<ConsultationDto?> GetByIdAsync(int id)
    {
        var consultation = await _consultationRepository.GetByIdWithMeasurementAsync(id);
        if (consultation == null) return null;

        return new ConsultationDto
        {
            Id = consultation.Id,
            Date = consultation.Date,
            Reason = consultation.Reason,
            Description = consultation.Description,
            PatientId = consultation.PatientId,
            Measurement = consultation.Measurement == null ? null : new MeasurementDto
            {
                Id = consultation.Measurement.Id,
                Weight = consultation.Measurement.Weight,
                Height = consultation.Measurement.Height,
                Size = consultation.Measurement.Size,
                IMC = consultation.Measurement.IMC
            }
        };
    }

    public async Task AddAsync(CreateConsultationDto dto)
    {
        var consultation = new Consultation
        {
            Date = dto.Date,
            Reason = dto.Reason,
            Description = dto.Description,
            PatientId = dto.PatientId,
        };
        if (dto.Measurement != null)
        {
            double weight = dto.Measurement.Weight;
            double height = dto.Measurement.Height;
            double imc = CalculateImc(weight, height);

            

            consultation.Measurement = new Measurement
            {
                Weight = dto.Measurement.Weight,
                Height = dto.Measurement.Height,
                Size = dto.Measurement.Size,
                IMC = imc
            };
        }
        await _consultationRepository.AddAsync(consultation);
    }

    public async Task UpdateAsync(int id, UpdateConsultationDto dto)
    {
        var consultation = await _consultationRepository.GetByIdAsync(id);
        if (consultation == null) throw new InvalidOperationException("Consultation not found");

        consultation.Date = dto.Date;
        consultation.Reason = dto.Reason;
        consultation.Description = dto.Description;
        consultation.PatientId = dto.PatientId;

        if (dto.Measurement != null)
        {
            double weight = dto.Measurement.Weight;
            double height = dto.Measurement.Height;
            double imc = CalculateImc(weight, height);

            if (consultation.Measurement == null)
            {
                consultation.Measurement = new Measurement();
            }

            consultation.Measurement.Weight = dto.Measurement.Weight;
            consultation.Measurement.Height = dto.Measurement.Height;
            consultation.Measurement.Size = dto.Measurement.Size;
            consultation.Measurement.IMC = imc;
        }
        else
        {
            consultation.Measurement = null;
        }

        await _consultationRepository.UpdateAsync(consultation);
    }

    public async Task DeleteAsync(int id)
    {
        var consultation = await _consultationRepository.GetByIdAsync(id);
        if (consultation == null) throw new InvalidOperationException("Consultation not found");

        await _consultationRepository.DeleteAsync(consultation.Id);
    }
}