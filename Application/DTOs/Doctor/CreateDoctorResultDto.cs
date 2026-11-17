namespace Application.DTOs.Doctor;

public class CreateDoctorResultDto : DoctorDto
{
    public bool LinkedToExistingAccount { get; set; }
}