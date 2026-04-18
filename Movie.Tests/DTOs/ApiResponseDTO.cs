using System.Text.Json.Serialization;

namespace Movies.Tests.DTOs
{
    public class ApiResponseDto
    {
        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("movie")]
        public MovieDto Movie { get; set; }
    }
}
