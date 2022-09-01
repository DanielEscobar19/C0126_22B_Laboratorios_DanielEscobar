// Daniel Escobar Giraldo | C02748

using Laboratorio3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Laboratorio3.Controllers
{
    public class SongController : Controller
    {
        public IActionResult Index()
        {
            var song = this.FavoriteSong();
            ViewBag.MainTitle = "My favorite song:";
            return View(song);
        }

        private SongModel FavoriteSong()
        {
            return new SongModel
            {
                Id = 1,
                Name = "Take it",
                Artist = "Dom Dolla",
                Genre = "Techno",
                ReleaseDate = new DateTime(2018, 7, 27),
                Duration = new TimeSpan(0, 3, 54)
            };
        }
    }
}
