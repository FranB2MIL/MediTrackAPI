namespace Application.DTOs.Doctor;

public class DoctorDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
}