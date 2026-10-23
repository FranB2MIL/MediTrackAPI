using System.Globalization;
using Application.Common;
using Application.DTOs.Appointment;
using Application.DTOs.Consultation;
using Application.DTOs.Doctor;
using Application.DTOs.Patient;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Enums;
using Domain.Interfaces;

namespace Application.Services;

public class PatientPortalService : IPatientPortalService
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAvailabilityRepository _availabilityRepository;

    public PatientPortalService(IConsultationRepository consultationRepository, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IAppointmentRepository appointmentRepository, IAvailabilityRepository availabilityRepository)
    {
        _consultationRepository = consultationRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _availabilityRepository = availabilityRepository;
    }

    public async Task<IEnumerable<PatientConsultationDto>> GetConsultationsAsync(int patientId, int? doctorId)
    {
        var consultations = await _consultationRepository.GetByPatientIdAsync(patientId, doctorId);
        return consultations.Select(MapToConsultationDto);
    }

    public async Task<IEnumerable<DoctorSummaryDto>> GetDoctorsAsync(int patientId)
    {
        var doctors = await _doctorRepository.GetByPatientIdAsync(patientId);
        return doctors.Select(MapToDoctorSummaryDto);
    }

    public async Task<PatientProfileDto?> GetProfileAsync(int patientId)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId);
        return patient == null ? null : MapToProfileDto(patient);
    }

    public async Task<PatientProfileDto?> UpdateProfileAsync(int patientId, UpdateMyProfileDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId);
        if (patient == null) return null;

        patient.FirstName = dto.FirstName;
        patient.LastName = dto.LastName;
        patient.DateOfBirth = dto.DateOfBirth!.Value;
        patient.HealthInsurance = dto.HealthInsurance;
        patient.InsuranceNumber = dto.InsuranceNumber;
        patient.PhoneNumber = dto.PhoneNumber;

        await _patientRepository.UpdateAsync(patient);
        return MapToProfileDto(patient);
    }

    public async Task<IEnumerable<AvailableSlotDto>?> GetAvailableSlotsAsync(int patientId, int doctorId, DateTime from, DateTime to)
    {
        if (!await _patientRepository.IsLinkedAsync(doctorId, patientId)) return null;

        var now = ArgentinaTime.Now();
        var start = from.Date < now.Date ? now.Date : from.Date;
        var end = to.Date;
        if (start > end) return Enumerable.Empty<AvailableSlotDto>();

        var availabilities = await _availabilityRepository.GetByDoctorIdAsync(doctorId);
        var taken = (await _appointmentRepository.GetActiveByDoctorAndRangeAsync(doctorId, start, end))
            .Select(a => (a.Date.Date, a.StartTime))
            .ToHashSet();

        return SlotGenerator.Generate(availabilities, start, end)
            .Where(s => !taken.Contains((s.Date, s.StartTime)))
            .Where(s => s.Date + s.StartTime.ToTimeSpan() > now)
            .Select(MapToSlotDto);
    }

    public async Task<(BookingResult Result, MyAppointmentDto? Appointment)> BookAsync(int patientId, CreateMyAppointmentDto dto)
    {
        var doctorId = dto.DoctorId!.Value;
        if (!await _patientRepository.IsLinkedAsync(doctorId, patientId)) return (BookingResult.NotFound, null);

        if (!TimeOnly.TryParse(dto.StartTime, CultureInfo.InvariantCulture, out var startTime))
            return (BookingResult.Invalid, null);

        var date = dto.Date!.Value.Date;
        if (date + startTime.ToTimeSpan() <= ArgentinaTime.Now()) return (BookingResult.Invalid, null);

        var availabilities = await _availabilityRepository.GetByDoctorIdAsync(doctorId);
        var slot = SlotGenerator.FindSlot(availabilities, date, startTime);
        if (slot == null) return (BookingResult.Invalid, null);

        var appointment = new Appointment
        {
            Date = slot.Date,
            StartTime = slot.StartTime,
            Status = AppointmentStatus.Reservado,
            AvailabilityId = slot.AvailabilityId,
            DoctorId = doctorId,
            PatientId = patientId
        };
        // Sin chequeo previo de "tomado": el índice único es la fuente de verdad ante reservas simultáneas.
        if (!await _appointmentRepository.TryAddAsync(appointment)) return (BookingResult.Conflict, null);

        var created = await _appointmentRepository.GetByIdAndPatientIdAsync(appointment.Id, patientId);
        return (BookingResult.Success, MapToMyAppointmentDto(created!));
    }

    public async Task<IEnumerable<MyAppointmentDto>> GetMyAppointmentsAsync(int patientId)
    {
        var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
        return appointments.Select(MapToMyAppointmentDto);
    }

    public async Task<UpdateResult> CancelMyAppointmentAsync(int id, int patientId)
    {
        var appointment = await _appointmentRepository.GetByIdAndPatientIdAsync(id, patientId);
        if (appointment == null) return UpdateResult.NotFound;

        var isPast = appointment.Date + appointment.StartTime.ToTimeSpan() <= ArgentinaTime.Now();
        if (appointment.Status == AppointmentStatus.Cancelado || isPast) return UpdateResult.Conflict;

        appointment.Status = AppointmentStatus.Cancelado;
        await _appointmentRepository.UpdateAsync(appointment);
        return UpdateResult.Success;
    }

    private static PatientConsultationDto MapToConsultationDto(Consultation consultation) => new PatientConsultationDto
    {
        Id = consultation.Id,
        Date = consultation.Date,
        Reason = consultation.Reason,
        Description = consultation.Description,
        Measurement = consultation.Measurement == null ? null : new MeasurementDto
        {
            Id = consultation.Measurement.Id,
            Weight = consultation.Measurement.Weight,
            Height = consultation.Measurement.Height,
            Size = consultation.Measurement.Size,
            IMC = consultation.Measurement.IMC
        },
        DoctorId = consultation.DoctorId,
        DoctorFirstName = consultation.Doctor.FirstName,
        DoctorLastName = consultation.Doctor.LastName
    };

    private static DoctorSummaryDto MapToDoctorSummaryDto(Doctor doctor) => new DoctorSummaryDto
    {
        Id = doctor.Id,
        FirstName = doctor.FirstName,
        LastName = doctor.LastName
    };

    private static PatientProfileDto MapToProfileDto(Patient patient) => new PatientProfileDto
    {
        Id = patient.Id,
        FirstName = patient.FirstName,
        LastName = patient.LastName,
        DateOfBirth = patient.DateOfBirth,
        DNI = patient.DNI,
        HealthInsurance = patient.HealthInsurance,
        InsuranceNumber = patient.InsuranceNumber,
        PhoneNumber = patient.PhoneNumber,
        Email = patient.Email
    };

    private static AvailableSlotDto MapToSlotDto(Slot slot) => new AvailableSlotDto
    {
        Date = slot.Date.ToString("yyyy-MM-dd"),
        StartTime = slot.StartTime.ToString("HH:mm"),
        EndTime = slot.EndTime.ToString("HH:mm"),
        AvailabilityId = slot.AvailabilityId
    };

    private static MyAppointmentDto MapToMyAppointmentDto(Appointment appointment) => new MyAppointmentDto
    {
        Id = appointment.Id,
        Date = appointment.Date.ToString("yyyy-MM-dd"),
        StartTime = appointment.StartTime.ToString("HH:mm"),
        EndTime = appointment.StartTime.AddMinutes(appointment.Availability.AppointmentDuration).ToString("HH:mm"),
        Status = appointment.Status.ToString(),
        DoctorId = appointment.DoctorId,
        DoctorFirstName = appointment.Doctor.FirstName,
        DoctorLastName = appointment.Doctor.LastName
    };
}
