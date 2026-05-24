namespace Application.DTOs.Doctor;

public class UpdateDoctorDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
}