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
            PatientId = c.PatientId,
            Measurement = c.Measurement == null ? null : new MeasurementDto
            {
                Id = c.Measurement.Id,
                Weight = c.Measurement.Weight,
                Height = c.Measurement.Height,
                Size = c.Measurement.Size,
                IMC = c.Measurement.IMC
            }
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

    public async Task AddAsync(CreateConsultationDto dto, int patientId)
    {
        var consultation = new Consultation
        {
            Date = dto.Date,
            Reason = dto.Reason,
            Description = dto.Description,
            PatientId = patientId,
        };
        if (dto.Measurement != null &&
            dto.Measurement.Weight > 0 &&
            dto.Measurement.Height > 0)
        {
            double weight = dto.Measurement.Weight ?? 0;
            double height = dto.Measurement.Height ?? 0;
            double size = dto.Measurement.Size ?? 0;
            double imc = CalculateImc(weight, height);



            consultation.Measurement = new Measurement
            {
                Weight = weight,
                Height = height,
                Size = size,
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

        if (dto.Measurement != null)
        {
            double weight = dto.Measurement.Weight ?? 0;
            double height = dto.Measurement.Height ?? 0;
            double size = dto.Measurement.Size ?? 0;
            double imc = CalculateImc(weight, height);

            if (consultation.Measurement == null)
            {
                consultation.Measurement = new Measurement();
            }

            consultation.Measurement.Weight = weight;
            consultation.Measurement.Height = height;
            consultation.Measurement.Size = size;
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