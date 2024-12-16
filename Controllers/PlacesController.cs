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

        [HttpPost]
        public async Task<IActionResult> SavePlace([FromBody] SavePlaceRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PlaceId))
            {
                return Json(new { success = false, message = "Place ID is required." });
            }

            // Verificar si el lugar ya existe en Places
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == request.PlaceId);

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

            // Retornar éxito sin depender de usuario autenticado
            return Json(new { success = true, message = "Place saved successfully!" });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SavePlaceFavorite([FromBody] SavePlaceRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PlaceId))
            {
                return Json(new { success = false, message = "Place ID is required." });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Verificar si el lugar ya existe en Places
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == request.PlaceId);

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
            var existingFavorite = await _context.SavedPlaces
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.PlaceId == request.PlaceId);

            if (existingFavorite != null)
            {
                return Json(new { success = false, message = "Place is already in favorites." });
            }

            // Guardar en favoritos
            var savedPlace = new SavedPlace
            {
                UserId = userId,
                PlaceId = request.PlaceId,
                SavedAt = DateTime.UtcNow
            };

            _context.SavedPlaces.Add(savedPlace);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Place added to favorites!" });
        }


        [Authorize]
        [HttpGet]
        public IActionResult GetFavorites()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var favorites = _context.SavedPlaces
                .Include(sp => sp.Place)
                .ThenInclude(p => p.Suggestions) // Incluir sugerencias relacionadas
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

            // Verificar si el lugar existe, si no, lo guardamos
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == request.PlaceId);

            if (place == null)
            {
                // Guardar el lugar con valores proporcionados o predeterminados
                place = new Place
                {
                    Id = request.PlaceId,
                    Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : "Unknown Place",
                    Distance = request.Distance > 0 ? request.Distance : 0,
                    Timezone = !string.IsNullOrWhiteSpace(request.Timezone) ? request.Timezone : "N/A"
                };

                _context.Places.Add(place);
                await _context.SaveChangesAsync();
            }

            // Guardar la sugerencia
            var suggestion = new Suggestion
            {
                UserId = User.Identity.IsAuthenticated
                    ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value) // Usuario autenticado
                    : 0, // Usuario anónimo
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
            public string? Name { get; internal set; }
            public int Distance { get; internal set; }
            public string? Timezone { get; internal set; }
        }

    }
}
