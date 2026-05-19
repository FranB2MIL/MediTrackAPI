namespace Domain.Entities
{
    public class Medicion
    {
        public int Id { get; set; }
        public decimal Peso { get; set; }
        public decimal Altura { get; set; }
        public decimal Talla { get; set; }
        public decimal IMC { get; set; }

        public int ConsultaId { get; set; }
        public Consulta Consulta { get; set; } = null!;
    }
}