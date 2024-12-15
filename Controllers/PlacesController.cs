using Microsoft.AspNetCore.Mvc;
using PlaceFinder.Models;
using PlaceFinder.Services;

namespace PlaceFinder.Controllers
{
    public class PlacesController : Controller
    {
        private readonly FoursquareService _foursquareService;

        public PlacesController(FoursquareService service)
        {
            _foursquareService = service;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? query, string? location)
        {

            if (string.IsNullOrWhiteSpace(query) || string.IsNullOrWhiteSpace(location))
            {
                ViewBag.Message = "Please provide both query and location.";
                return View(new List<Place>());
            }

            try
            {
                var places = await _foursquareService.SearchPlacesAsync(query, location);

                if (!places.Any())
                {
                    ViewBag.Message = "No places found. Try another search.";
                }

                return View(places);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View(new List<Place>());
            }
        }

    }
}
