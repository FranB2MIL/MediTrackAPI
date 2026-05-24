namespace Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaDeNacimiento { get; set; }
        public string DNI { get; set; } = string.Empty;
        public string ObraSocial { get; set; } = string.Empty;
        public string NumeroAfiliado { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string NumeroDeTelefono { get; set; } = string.Empty;

        public ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
        public ICollection<Study> Studies { get; set; } = new List<Study>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<DoctorPatient> DoctorPatients { get; set; } = new List<DoctorPatient>();

    }
}
