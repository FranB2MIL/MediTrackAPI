namespace Application.DTOs.Paciente
{
    public class CreatePatientDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string ObraSocial { get; set; } = string.Empty;
    }
}