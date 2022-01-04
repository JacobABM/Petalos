using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Petalos.Areas.Admin.Models;
using Petalos.Models;

namespace Petalos.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public floresContext Context { get; }
        public IWebHostEnvironment Host { get; }

        public HomeController(floresContext context, IWebHostEnvironment host)
        {
            Context = context;
            Host = host;
        }

        public IActionResult Index()
        {
            IEnumerable<Datosflores> flores = Context.Datosflores.OrderBy(x => x.Nombre);
            return View(flores);
        }

        public IActionResult AgregarFlor()
        {
            return View(new Datosflores());
        }

        [HttpPost]
        public IActionResult AgregarFlor(Datosflores datos)
        {
            if (string.IsNullOrWhiteSpace(datos.Nombre))
                ModelState.AddModelError("", "Por favor ingrese nombre de la flor");
            if (string.IsNullOrWhiteSpace(datos.Nombrecientifico))
                ModelState.AddModelError("", "Por favor ingrese nombre cientifico de la flor");
            if (string.IsNullOrWhiteSpace(datos.Nombrecomun))
                ModelState.AddModelError("", "Por favor ingrese nombre comun de la flor");
            if (string.IsNullOrWhiteSpace(datos.Origen))
                ModelState.AddModelError("", "Por favor ingrese origen de la flor");
            if (string.IsNullOrWhiteSpace(datos.Descripcion))
                ModelState.AddModelError("", "Por favor ingrese descripcion de la flor");
            if (Context.Datosflores.Any(x => x.Nombre == datos.Nombre))
                ModelState.AddModelError("", "Ya existe una flor con ese nombre");
            if (ModelState.IsValid)
            {
                Context.Datosflores.Add(datos);
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(datos);
        }

        public IActionResult AgregarImagenes(int id)
        {
            IEnumerable<Imagenesflores> imagenes = Context.Imagenesflores.Where(x => x.Idflor == id);
            Datosflores datos = Context.Datosflores.FirstOrDefault(x => x.Idflor == id);
            return View(new FloresImagenesViewModel { Datos = datos, Imagenes = imagenes });
        }

        [HttpPost]
        public IActionResult AgregarImagenes(Imagenesflores datos, IFormFile img)
        {
            if (img != null)
            {
                var path = Host.WebRootPath + "/images/" + img.FileName.Replace("-", " ");
                FileStream fs = new FileStream(path, FileMode.Create);
                img.CopyTo(fs);
                fs.Close();
            }
            else
            {
                ModelState.AddModelError("", "No se ha seleccionado ninguna imagen a cargar");
            }
            if (datos.Idflor == 0)
            {
                ModelState.AddModelError("", "No hay ninguna flor relacionada para agregar");
            }

            if (ModelState.IsValid)
            {
                datos.Idimagen = 0;
                datos.Nombreimagen = img.FileName.Replace("-", " ");
                Context.Imagenesflores.Add(datos);
                Context.SaveChanges();
            }
            return View(new FloresImagenesViewModel {Datos=Context.Datosflores.FirstOrDefault(x=>x.Idflor==datos.Idflor), Imagenes=Context.Imagenesflores.Where(x=>x.Idflor==datos.Idflor)});
        }

        public IActionResult EliminarImagen(string id)
        {
            string nombre = id.Replace("-", " ");
            Imagenesflores imagen = Context.Imagenesflores.FirstOrDefault(x => x.Nombreimagen == nombre);
            if (imagen != null)
            {
                Context.Imagenesflores.Remove(imagen);
                Context.SaveChanges();

                var path = Host.WebRootPath + "/images/" + imagen.Nombreimagen;
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return RedirectToAction("AgregarImagenes", new { id = imagen.Idflor });
            }
            return RedirectToAction("Index");
        }

        public IActionResult EditarFlor(int id)
        {
            Datosflores flor = Context.Datosflores.FirstOrDefault(x => x.Idflor == id);
            if (flor == null)
                return RedirectToAction("Index");
            return View(flor);
        }

        [HttpPost]
        public IActionResult EditarFlor(Datosflores datos)
        {
            Datosflores flor = Context.Datosflores.FirstOrDefault(x => x.Idflor == datos.Idflor);
            if (flor == null)
                ModelState.AddModelError("", "No se encontro la flor a editar");
            if (string.IsNullOrWhiteSpace(datos.Nombre))
                ModelState.AddModelError("", "Por favor ingrese nombre de la flor");
            if (string.IsNullOrWhiteSpace(datos.Nombrecientifico))
                ModelState.AddModelError("", "Por favor ingrese nombre cientifico de la flor");
            if (string.IsNullOrWhiteSpace(datos.Nombrecomun))
                ModelState.AddModelError("", "Por favor ingrese nombre comun de la flor");
            if (string.IsNullOrWhiteSpace(datos.Origen))
                ModelState.AddModelError("", "Por favor ingrese origen de la flor");
            if (string.IsNullOrWhiteSpace(datos.Descripcion))
                ModelState.AddModelError("", "Por favor ingrese descripcion de la flor");
            if(Context.Datosflores.Any(x=>x.Nombre==datos.Nombre && x.Idflor != datos.Idflor))
                ModelState.AddModelError("", "Ya existe una flor con ese nombre");
            if (ModelState.IsValid)
            {
                flor.Nombre = datos.Nombre;
                flor.Nombrecientifico = datos.Nombrecientifico;
                flor.Nombrecomun = datos.Nombrecomun;
                flor.Origen = datos.Origen;
                flor.Descripcion = datos.Descripcion;
                Context.Datosflores.Update(flor);
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(datos);
        }

        public IActionResult Eliminar(string id)
        {
            var flor = Context.Datosflores.Include(x => x.Imagenesflores).FirstOrDefault(x => x.Nombre == id);
            if (flor != null)
            {
                Context.Remove(flor);
                Context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}