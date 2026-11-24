namespace Biblioteca.Web.Models
{
    public class Socio
    {
        /*El comentario agreado por medio del intellicen para probar y explicar el funcionamiento*/
        public const int MaximoLibrosPrestados = 3;  //constante que define el máximo de libros que un socio puede tener prestados al mismo tiempo.
        private readonly List<Prestamo> _prestamos = [];

        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool PuedePedirPrestamo(IEnumerable<Prestamo> prestamos)
        {
            // Cuenta cuántos préstamos activos tiene el socio (es decir, aquellos que no han sido devueltos).
            int prestamosActivos = prestamos
                .Count(prestamo => prestamo.SocioId == Id && prestamo.FechaDevolucion == null);
            // Devuelve true si el número de préstamos activos es menor que el máximo permitido, false en caso contrario.
            return prestamosActivos < MaximoLibrosPrestados;
        }

        public bool PuedePedir(int socioId)
        {
            int activos = _prestamos.Count(p => p.SocioId == socioId);
            return activos <= MaximoLibrosPrestados;

        }
        /* Errores de PuedePedir */
    }

}
