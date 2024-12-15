using System.Text.Json;
using PlaceFinder.Models;

namespace PlaceFinder.Services
{
    public class FoursquareService
    {
        private readonly HttpClient _httpClient;

        public FoursquareService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Place>> SearchPlacesAsync(string query, string location)
        {
            try
            {
                var response = await _httpClient.GetAsync($"places/search?query={query}&near={location}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var result = JsonSerializer.Deserialize<FoursquareResponse>(jsonResponse, options);
                    return result?.Results ?? new List<Place>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data from Foursquare: {ex.Message}");
            }

            return new List<Place>();
        }
    }

    public class FoursquareResponse
    {
        public List<Place> Results { get; set; }
    }
}
