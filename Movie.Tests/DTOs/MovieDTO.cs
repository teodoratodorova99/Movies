using System.Text.Json.Serialization;

namespace Movies.Tests.DTOs
{
    public class MovieDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("posterUrl")]
        public string PosterUrl { get; set; }

        [JsonPropertyName("trailerLink")]
        public string TrailerLink { get; set; }

        [JsonPropertyName("isWatched")]
        public bool IsWatched { get; set; }
    }
}
