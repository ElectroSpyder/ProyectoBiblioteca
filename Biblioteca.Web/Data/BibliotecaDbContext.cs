using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Data;

public sealed class BibliotecaDbContext : DbContext
{
    public BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Libro> Libros => Set<Libro>();
}
