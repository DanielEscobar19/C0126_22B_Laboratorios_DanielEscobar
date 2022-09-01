// Daniel Escobar Giraldo | C02748

using Laboratorio3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Laboratorio3.Controllers
{
    public class MovieController : Controller
    {
        public IActionResult Index()
        {
            var movies = GetListOfMovies();
            ViewBag.MainTitle = "These are my favorite movies";
            return View(movies);
        }
        private List<MovieModel> GetListOfMovies()
        {
            List<MovieModel> movies = new List<MovieModel>();
            movies.Add(new MovieModel
            {
                Id = 1,
                Name = "Pulp Fiction",
                Genre = "Crime/Drama",
                ReleaseDate = new DateTime(1994, 10, 14)
            });
            movies.Add(new MovieModel
            {
                Id = 2,
                Name = "Toy Store",
                Genre = "Family/Comedy",
                ReleaseDate = new DateTime(1995, 11, 22)
            });
            movies.Add(new MovieModel
            {
                Id = 3,
                Name = "Mulan",
                Genre = "Family/Comedy",
                ReleaseDate = new DateTime(1998, 06, 19)
            });
            return movies;
        }
    }
}
