using Microsoft.AspNetCore.Mvc;
using PlaceFinder.Models;
using PlaceFinder.Services;

namespace PlaceFinder.Controllers
{
    public class PlacesController : Controller
    {
        private readonly FoursquareService _foursquareService;

        public PlacesController(FoursquareService foursquareService)
        {
            _foursquareService = foursquareService;
        }

        public async Task<IActionResult> Search(string query, string location)
        {
            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(location))
            {
                ViewBag.Message = "Both query and location are required.";
                return View(new List<Place>());
            }

            var places = await _foursquareService.SearchPlacesAsync(query, location);
            return View(places); 
        }

    }
}
