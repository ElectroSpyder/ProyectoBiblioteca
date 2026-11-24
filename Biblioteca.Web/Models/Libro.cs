namespace Biblioteca.Web.Models;

    // Modelo de la Biblioteca: representa un libro.
    // Es una clase común (no un record) porque más adelante vamos a
    // modificar propiedades (por ejemplo, marcar un libro como no disponible)
    // y Entity Framework necesita propiedades con set.
    public class Libro
    {
        // Identificador único del libro
        public int Id { get; set; }

        // Título del libro (arranca vacío para evitar valores nulos)
        public string Titulo { get; set; } = string.Empty;

        // Autor del libro
        public string Autor { get; set; } = string.Empty;

        // Año de publicación
        public int Anio { get; set; }
    }

