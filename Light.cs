using System.Text.Json.Serialization;

namespace lightsmeai.Models
{
    public class Light
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("Switched")]
        public bool? Switched { get; set; }
    }
}