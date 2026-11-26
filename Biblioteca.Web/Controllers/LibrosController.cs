using Biblioteca.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Web.Controllers;

public class LibrosController(BibliotecaDbContext context) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var libros = await context.Libros
            .AsNoTracking()
            .OrderBy(libro => libro.Titulo)
            .ThenBy(libro => libro.Id)
            .ToListAsync(cancellationToken);

        return View(libros);
    }
}
