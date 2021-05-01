using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Movies.Client.Services
{
    public class CRUDService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();

        public CRUDService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);

            _httpClient.DefaultRequestHeaders.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(
            //    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            //_httpClient.DefaultRequestHeaders.Accept.Add(
            //    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml",0.9));

        }

        public async Task Run()
        {
            //await GetResource();

            //await GetResourceThroughHttpRequestMessage();

            //await CreateResource();

            //await UpdateResource();

            await DeleteResource();
        }

        public async Task GetResource()
        {
            var response = await _httpClient.GetAsync("api/movies");

            response.EnsureSuccessStatusCode(); //Throws exception if response not successful

            var content = await response.Content.ReadAsStringAsync();

            //var movies = JsonConvert.DeserializeObject<IEnumerable<Movie>>(content);

            var movies = new List<Movie>();

            if(response.Content.Headers.ContentType.MediaType == "application/json")
            {
                movies = JsonConvert.DeserializeObject<List<Movie>>(content);
            }
            else if(response.Content.Headers.ContentType.MediaType == "application/xml")
            {
                var serializer = new XmlSerializer(typeof(List<Movie>));
                movies = (List<Movie>)serializer.Deserialize(new StringReader(content));
            }
        }

        public async Task GetResourceThroughHttpRequestMessage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");

            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<List<Movie>>(content);

        }

        public async Task CreateResource()
        {
            var movieToCreate = new MovieForCreation()
            {
                Title = "Reservior Dogs",
                Description = "sdfgsdf sfgsdfg",
                DirectorId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var serializedMovieToCreate = JsonConvert.SerializeObject(movieToCreate);

            var request = new HttpRequestMessage(HttpMethod.Post, "api/movies");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializedMovieToCreate);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var createdMovie = JsonConvert.DeserializeObject<Movie>(content);
        }

        public async Task UpdateResource()
        {
            var movieToUpdate = new MovieForUpdate()
            {
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                Title = "Pulp Fiction",
                Description = "The Movie with Zed.",
                ReleaseDate = new DateTimeOffset(new DateTime(1994, 11, 9)),
                Genre = "Crime, Drama"
            };

            var serializedMovieToUpdate = JsonConvert.SerializeObject(movieToUpdate);

            var request = new HttpRequestMessage(HttpMethod.Put, "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            request.Content = new StringContent(serializedMovieToUpdate);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);
        }

        public async Task DeleteResource()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/movies/6e87f657-f2c1-4d90-9b37-cbe43cc6adb9");
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

        }

        public async Task PostResourceShortcut()
        {
            var movieToCreate = new MovieForCreation()
            {
                Title = "Reservior Dogs II",
                Description = "sdfgsdf sfgsdfg",
                DirectorId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var response = await _httpClient.PostAsync("api/movies",
                new StringContent(JsonConvert.SerializeObject(movieToCreate), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var createdMovie = JsonConvert.DeserializeObject<Movie>(content);

        }
    }
}
