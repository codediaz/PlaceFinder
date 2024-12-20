﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PlaceFinder.Models
{
    [NotMapped]
    public class Category
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("icon")]
        public Icon Icon { get; set; } = new Icon();

        public string IconUrl => $"{Icon.Prefix}bg_64{Icon.Suffix}";
    }


}