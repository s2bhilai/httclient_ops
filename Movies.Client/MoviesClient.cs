using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client
{
    public class MoviesClient
    {
        private HttpClient _httpClient;

        public MoviesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task<IEnumerable<Movie>> GetMovies(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/movies");
            request.Headers.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding
                .Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            using(var response = await _httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();

                return stream.ReadAndDeserializeFromJson<List<Movie>>();

            }
        }
    }
}
