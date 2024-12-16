using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaceFinder.Data;
using PlaceFinder.Models;
using PlaceFinder.Services;

namespace PlaceFinder.Controllers
{
    public class PlacesController : Controller
    {
        private readonly FoursquareService _foursquareService;
        private readonly MyDbContext _context;

        public PlacesController(FoursquareService service, MyDbContext context)
        {
            _foursquareService = service;
            _context = context;
        }

        public class SavePlaceRequest
        {
            public string PlaceId { get; set; }
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SavePlace([FromBody] SavePlaceRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PlaceId))
            {
                return Json(new { success = false, message = "Place ID is required." });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Verificar si el lugar ya existe en Places
            var place = _context.Places.FirstOrDefault(p => p.Id == request.PlaceId);

            if (place == null)
            {
                // Insertar el lugar en la tabla Places si no existe
                place = new Place
                {
                    Id = request.PlaceId,
                    Name = "Unknown", // Por defecto, hasta obtener más detalles
                    Distance = 0,
                    Timezone = "Unknown"
                };

                _context.Places.Add(place);
                await _context.SaveChangesAsync();
            }

            // Verificar si ya existe en favoritos
            if (_context.SavedPlaces.Any(sp => sp.UserId == userId && sp.PlaceId == request.PlaceId))
            {
                return Json(new { success = false, message = "Place is already saved." });
            }

            // Insertar el lugar en SavedPlaces
            var savedPlace = new SavedPlace
            {
                UserId = userId,
                PlaceId = request.PlaceId,
                SavedAt = DateTime.UtcNow
            };

            _context.SavedPlaces.Add(savedPlace);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Place saved successfully!" });
        }


        [Authorize]
        [HttpGet]
        public IActionResult GetFavorites()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var favorites = _context.SavedPlaces
                .Include(sp => sp.Place)
                .Where(sp => sp.UserId == userId)
                .Select(sp => sp.Place)
                .ToList();

            return PartialView("_FavoritesList", favorites);
        }
    }
}
