/*
    Daniel Escobar Giraldo | C02748
 */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Laboratorio5.Handlers;
using Laboratorio5.Models;

namespace Laboratorio5.Controllers
{
    public class PeliculasController : Controller
    {
        public IActionResult Index()
        {
            PeliculasHandler peliculasHandler = new PeliculasHandler();
            var peliculas = peliculasHandler.ObtenerPeliculas();
            ViewBag.MainTitle = "Lista de Peliculas";
            if (TempData["MensajeError"] != null)
            {
                ViewBag.MensajeError = TempData["MensajeError"];
            }
            return View(peliculas);
        }

        [HttpGet]
        public ActionResult CrearPelicula()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearPelicula(PeliculaModel pelicula)
        {
            ViewBag.ExitoAlCrear = false;
            try
            {
                if (ModelState.IsValid)
                {
                    PeliculasHandler peliculasHandler = new PeliculasHandler();
                    ViewBag.ExitoAlCrear = peliculasHandler.CrearPelicula(pelicula);
                    if (ViewBag.ExitoAlCrear)
                    {
                        ViewBag.Message = "La Pelicula " + pelicula.Nombre + " fue creada con éxito.";
                        ModelState.Clear();
                    }
                }
                return View();
            }
            catch
            {
                ViewBag.Message = "Algo salió mal y no fue posible crear la película";
                return View();
            }
        }
        public ActionResult EditarPelicula(int? identificador)
        {
            ActionResult vista;
            TempData.Clear();
            try
            {
                PeliculasHandler peliculasHandler = new PeliculasHandler();
                var pelicula = peliculasHandler.ObtenerPeliculas().Find(model => model.ID == identificador);
                if (pelicula == null)
                {
                    TempData["EditSuccess"] = @"El identificador @identificador no coincide con ninguna película";
                    vista = RedirectToAction("Index");
                }
                else
                {
                    TempData["EditFail"] = @"La película @pelicula.Nombre fue editada con éxito";
                    vista = View(pelicula);
                }
            }
            catch
            {
                vista = RedirectToAction("Index");
            }
            return vista;
        }

        [HttpPost]
        public ActionResult EditarPelicula(PeliculaModel pelicula)
        {
            try
            {
                PeliculasHandler peliculasHandler = new PeliculasHandler();
                peliculasHandler.EditarPelicula(pelicula);
                return RedirectToAction("Index", "Peliculas");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult BorrarPelicula(int? identificador)
        {
            try
            {
                PeliculasHandler peliculasHandler = new PeliculasHandler();
                var pelicula = peliculasHandler.ObtenerPeliculas().Find(model => model.ID == identificador);
                if (pelicula == null)
                {
                    TempData["MensajeError"] = @"El identificador @identificador no coincide con ninguna película";
                }
                else
                {
                    peliculasHandler.BorrarPelicula(pelicula);
                    TempData["MensajeError"] = "La película " + pelicula.Nombre + " ha sido borrada con éxito";
                }
            }
            catch
            {
                TempData["MensajeError"] = @"El identificador @identificador no coincide con ninguna película";
            }
            return RedirectToAction("Index");
        }
        /*
            Daniel Escobar Giraldo | C02748
        */

    }
}
