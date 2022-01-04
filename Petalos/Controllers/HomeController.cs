using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Petalos.Models;
using Petalos.Models.ViewModels;

namespace Petalos.Controllers
{
    public class HomeController : Controller
    {
        public floresContext Context { get; }

        public HomeController(floresContext context)
        {
            Context = context;
        }

        public IActionResult Index()
        {
            var flores = Context.Datosflores.OrderBy(x=>x.Nombre);
            return View(flores);
        }

        [Route("/Flor/{nombre}")]
        public IActionResult Flor(string nombre)
        {
            Datosflores flor = Context.Datosflores.FirstOrDefault(x=>x.Nombre==nombre);
            if (flor == null)
                return RedirectToAction("Index");

            IEnumerable<Datosflores> flores = Context.Datosflores.OrderBy(x => x.Nombre).Where(x=>x.Nombre!=nombre);
            IEnumerable<Imagenesflores> imagenes = Context.Imagenesflores.Where(x => x.Idflor == flor.Idflor);
            return View(new FlorViewModel {Datos=flor, Imagenes=imagenes, Flores=flores });
        }
    }
}