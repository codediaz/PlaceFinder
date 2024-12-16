using System.Text.Json;
using PlaceFinder.Models;

namespace PlaceFinder.Services
{
    public class FoursquareService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public FoursquareService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = Environment.GetEnvironmentVariable("FOURSQUARE_API_KEY")
                    ?? throw new Exception("API Key is missing or invalid.");
        }

        public async Task<List<Place>> SearchPlacesAsync(string query, string location)
        {
            try
            {
                var encodedQuery = Uri.EscapeDataString(query);
                var encodedLocation = Uri.EscapeDataString(location);
                var requestUri = $"https://api.foursquare.com/v3/places/search?query={encodedQuery}&near={encodedLocation}";
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

                request.Headers.TryAddWithoutValidation("Authorization", $"{_apiKey}");
                request.Headers.TryAddWithoutValidation("Accept", "application/json");

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<FoursquareResponse>(jsonResponse, options);
                    return result?.Results ?? new List<Place>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.ReasonPhrase}, Content: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return new List<Place>();
        }
    }

    public class FoursquareResponse
    {
        public List<Place> Results { get; set; } = new List<Place>();
    }
}
