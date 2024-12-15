using System.Text.Json.Serialization;

namespace PlaceFinder.Models
{
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

        [JsonPropertyName("geocodes")]
        public Geocodes Geocodes { get; set; } = new Geocodes();
    }

    public class Geocodes
    {
        [JsonPropertyName("main")]
        public MainCoordinates Main { get; set; } = new MainCoordinates();
    }

    public class MainCoordinates
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
