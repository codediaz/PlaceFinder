using System.Text.Json.Serialization;

namespace PlaceFinder.Models
{
    public class Place
    {
        [JsonPropertyName("fsq_id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("distance")]
        public int Distance { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("categories")]
        public List<Category?>? Categories { get; set; }


    }
}
