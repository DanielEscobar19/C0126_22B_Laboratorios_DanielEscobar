// Daniel Escobar Giraldo | C02748

using Microsoft.AspNetCore.Mvc;

namespace Laboratorio3.Controllers
{
    public class MovieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
