using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        [JsonPropertyName("categories")]
        public List<Category?>? Categories { get; set; }

        [NotMapped]
        [JsonPropertyName("geocodes")]
        public Geocodes Geocodes { get; set; } = new Geocodes();

        public ICollection<SavedPlace> SavedPlaces { get; set; } = new List<SavedPlace>();

        public ICollection<Suggestion> Suggestions { get; set; } = new List<Suggestion>();
    }

    [NotMapped]
    public class Geocodes
    {
        [JsonPropertyName("main")]
        public MainCoordinates Main { get; set; } = new MainCoordinates();
    }

   [NotMapped]
    public class MainCoordinates
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
