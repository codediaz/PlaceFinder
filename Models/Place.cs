using System.Text.Json.Serialization;

namespace PlaceFinder.Models
{
    public class Geocode
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Place
    {
        [JsonPropertyName("fsq_id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("distance")]
        public int Distance { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [JsonPropertyName("categories")]
        public List<Category?>? Categories { get; set; }

        public ICollection<SavedPlace>? SavedPlaces { get; set; }

        public Geocode Geocodes { get; set; }

    }
}
