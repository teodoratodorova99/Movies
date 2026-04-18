using Movies.Tests.DTOs;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace Movies
{
    public class MoviesTests
    {
        private RestClient client;
        private static string movieId;

        [OneTimeSetUp]
        public void Setup()
        {
            string jwtToken = GetJwtToken("tedora99@mail.com", "123456");

            RestClientOptions options = new RestClientOptions("http://144.91.123.158:5000")
            {
                Authenticator = new JwtAuthenticator(jwtToken)
            };

            client = new RestClient(options);
        }

        private string GetJwtToken(string email, string password)
        {
            RestClient client = new RestClient("http://144.91.123.158:5000");
            RestRequest request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new { email, password });

            RestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
                return json.GetProperty("accessToken").GetString();
            }

            throw new InvalidOperationException("Authentication failed.");
        }

        // -----------------------------
        // 1. CREATE MOVIE
        // -----------------------------
        [Order(1)]
        [Test]
        public void Test_CreateMovie()
        {
            var request = new RestRequest("/api/Movie/Create", Method.Post);

            var movie = new MovieDto
            {
                Title = "Test Movie",
                Description = "Test Description",
                PosterUrl = "",
                TrailerLink = "",
                IsWatched = true
            };

            request.AddJsonBody(movie);

            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);

            Assert.That(content.Movie, Is.Not.Null);
            Assert.That(content.Movie.Id, Is.Not.Empty);
            Assert.That(content.Msg, Is.EqualTo("Movie created successfully!"));

            movieId = content.Movie.Id;
        }

        // -----------------------------
        // 2. EDIT MOVIE
        // -----------------------------
        [Order(2)]
        [Test]
        public void Test_EditMovie()
        {
            var request = new RestRequest($"/api/Movie/Edit?movieId={movieId}", Method.Put);

            var editedMovie = new MovieDto
            {
                Title = "Edited Movie",
                Description = "Edited Description",
                PosterUrl = "",
                TrailerLink = "",
                IsWatched = false
            };

            request.AddJsonBody(editedMovie);

            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);
            Assert.That(content.Msg, Is.EqualTo("Movie edited successfully!"));
        }

        // -----------------------------
        // 3. GET ALL MOVIES
        // -----------------------------
        [Order(3)]
        [Test]
        public void Test_GetAllMovies()
        {
            var request = new RestRequest("/api/Catalog/All", Method.Get);

            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var movies = JsonSerializer.Deserialize<List<MovieDto>>(response.Content);

            Assert.That(movies, Is.Not.Null);
            Assert.That(movies.Count, Is.GreaterThan(0));
        }

        // -----------------------------
        // 4. DELETE MOVIE
        // -----------------------------
        [Order(4)]
        [Test]
        public void Test_DeleteMovie()
        {
            var request = new RestRequest($"/api/Movie/Delete?movieId={movieId}", Method.Delete);

            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var content = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);
            Assert.That(content.Msg, Is.EqualTo("Movie deleted successfully!"));
        }

        // -----------------------------
        // 5. CREATE MOVIE WITHOUT REQUIRED FIELDS
        // -----------------------------
        [Order(5)]
        [Test]
        public void Test_CreateMovie_MissingRequiredFields()
        {
            var request = new RestRequest("/api/Movie/Create", Method.Post);

            var invalidMovie = new MovieDto
            {
                Title = "",
                Description = ""
            };

            request.AddJsonBody(invalidMovie);

            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        // -----------------------------
        // 6. EDIT NON-EXISTING MOVIE
        // -----------------------------
        [Order(6)]
        [Test]
        public void Test_EditNonExistingMovie()
        {
            var request = new RestRequest("/api/Movie/Edit?movieId=invalid123", Method.Put);

            var movie = new MovieDto
            {
                Title = "Doesn't matter",
                Description = "Doesn't matter"
            };

            request.AddJsonBody(movie);

            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var content = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);
            Assert.That(content.Msg, Is.EqualTo("Unable to edit the movie! Check the movieId parameter or user verification!"));
        }

        // -----------------------------
        // 7. DELETE NON-EXISTING MOVIE
        // -----------------------------
        [Order(7)]
        [Test]
        public void Test_DeleteNonExistingMovie()
        {
            var request = new RestRequest("/api/Movie/Delete?movieId=invalid123", Method.Delete);

            var response = client.Execute(request);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            var content = JsonSerializer.Deserialize<ApiResponseDto>(response.Content);
            Assert.That(content.Msg, Is.EqualTo("Unable to delete the movie! Check the movieId parameter or user verification!"));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            client?.Dispose();
        }
    }
}
