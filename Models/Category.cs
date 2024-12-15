using System.Text.Json.Serialization;

namespace PlaceFinder.Models
{
    public class Category
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public Icon Icon { get; set; } 

        public string IconUrl => $"{Icon.Prefix}bg_64{Icon.Suffix}";
    }


}