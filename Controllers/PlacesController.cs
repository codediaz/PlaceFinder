using System.Text.Json;
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
                return Json(new { success = false, message = "Please provide both query and location." });
            }

            try
            {
                var places = await _foursquareService.SearchPlacesAsync(query, location);

                if (!places.Any())
                {
                    return Json(new { success = false, message = "No places found." });
                }

                return Json(new { success = true, data = places });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
