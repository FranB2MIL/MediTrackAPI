namespace Application.DTOs.Doctor;

public class CreateDoctorDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
}