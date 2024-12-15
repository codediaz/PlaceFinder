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
                TempData["Message"] = "Please provide both query and location.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
           
                var places = await _foursquareService.SearchPlacesAsync(query, location);

                if (!places.Any())
                {
                    TempData["Message"] = "No places found. Try another search.";
                }

                TempData["Results"] = JsonSerializer.Serialize(places);
            }
            catch (Exception ex)
            {
          
                TempData["Error"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
