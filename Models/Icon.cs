using System.Text.Json.Serialization;

namespace PlaceFinder.Models
{
    public class Icon
    {
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; } 

        [JsonPropertyName("suffix")]
        public string Suffix { get; set; } 
    }
}

