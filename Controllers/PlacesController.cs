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
            public string Name { get; set; }
            public int Distance { get; set; }
            public string Timezone { get; set; }
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
                // Insertar el lugar con los detalles completos
                place = new Place
                {
                    Id = request.PlaceId,
                    Name = request.Name,
                    Distance = request.Distance,
                    Timezone = request.Timezone
                };

                _context.Places.Add(place);
                await _context.SaveChangesAsync();
            }

            // Verificar si ya existe en favoritos
            if (_context.SavedPlaces.Any(sp => sp.UserId == userId && sp.PlaceId == request.PlaceId))
            {
                return Json(new { success = false, message = "Place is already saved." });
            }

            // Insertar en SavedPlaces
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

        [HttpGet]
        public IActionResult GetSuggestions(string placeId)
        {
            if (string.IsNullOrWhiteSpace(placeId))
            {
                return Json(new { success = false, message = "Place ID is required." });
            }

            var suggestions = _context.Suggestions
                .Where(s => s.PlaceId == placeId)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new
                {
                    s.Content,
                    s.UserId,
                    s.CreatedAt
                })
                .ToList();

            return Json(new { success = true, data = suggestions });
        }

        [HttpPost]
        public async Task<IActionResult> AddSuggestion([FromBody] SuggestionRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Content) || string.IsNullOrWhiteSpace(request.PlaceId))
            {
                return Json(new { success = false, message = "Invalid suggestion data." });
            }

            // Determinar el userId
            int userId = User.Identity.IsAuthenticated
                ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                : 0; // Usuario anónimo

            var suggestion = new Suggestion
            {
                UserId = userId,
                PlaceId = request.PlaceId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Suggestions.Add(suggestion);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Suggestion added successfully!" });
        }


        public class SuggestionRequest
        {
            public string PlaceId { get; set; }
            public string Content { get; set; }
        }

    }
}
