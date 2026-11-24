namespace Biblioteca.Web.Models
{
    public class Prestamo
    {
        public int Id { get; set; }

        public int LibroId { get; set; }

        public int SocioId { get; set; }

        public DateTime FechaPrestamo { get; set; }

        public DateTime? FechaDevolucion { get; set; }
    }
}
