using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PlaceFinder.Models;

namespace PlaceFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Search(string query, string location)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(location))
            {
                ViewBag.Message = "Please provide both query and location.";
                return View("Index"); // Devuelve la vista vacía
            }

            // Redirigir al controlador de Places con los parámetros
            return RedirectToAction("Search", "Places", new { query, location });
        }

        public IActionResult Index()
        {
            // Leer mensajes y errores desde TempData
            ViewBag.Message = TempData["Message"] as string;
            ViewBag.Error = TempData["Error"] as string;

            // Leer y deserializar los resultados
            var resultsJson = TempData["Results"] as string;
            IEnumerable<Place> results = null;

            if (!string.IsNullOrEmpty(resultsJson))
            {
                results = JsonSerializer.Deserialize<IEnumerable<Place>>(resultsJson);
            }

            return View(results);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
