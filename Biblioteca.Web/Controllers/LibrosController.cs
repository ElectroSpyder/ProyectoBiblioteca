using Biblioteca.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Biblioteca.Web.Controllers
{
    public class LibrosController : Controller
    {
        public async Task<IActionResult> Index()
        {

           // var libros = await new LibroRepository().GetAllAsync();

            return View(new List<Libro>());
        }
    }
}
