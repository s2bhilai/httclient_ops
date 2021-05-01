using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {
        private IHttpClientFactory _httpClientFactory;
        private MoviesClient _moviesClient;

        public HttpClientFactoryInstanceManagementService(IHttpClientFactory httpClientFactory,
            MoviesClient moviesClient)
        {
            _httpClientFactory = httpClientFactory;
            _moviesClient = moviesClient;
        }

        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        public async Task Run()
        {
            //await GetMoviesWithHttpClientFromFactory(_cancellationTokenSource.Token);

            //await GetMoviesWithNamedHttpClientFromFactory(_cancellationTokenSource.Token);

            // await GetMoviesWithTypedHttpClientFromFactory(_cancellationTokenSource.Token);

            await GetMoviesViaMoviesClient(_cancellationTokenSource.Token);
        }

        private async Task GetMoviesWithHttpClientFromFactory(
            CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get,
                "http://localhost:57863/api/movies");

            request.Headers.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            using(var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();

                var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();

            }
        }

        private async Task GetMoviesWithNamedHttpClientFromFactory(
            CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MoviesClient");

            var request = new HttpRequestMessage(HttpMethod.Get,"api/movies");

            request.Headers.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));


            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();

                var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
            }
        }


        //private async Task GetMoviesWithTypedHttpClientFromFactory(
        //    CancellationToken cancellationToken)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");

        //    request.Headers.Accept.Add(
        //        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        //    request.Headers.AcceptEncoding.Add(
        //        new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));


        //    using (var response = await _moviesClient._httpClient.SendAsync(request,
        //        HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        //    {
        //        response.EnsureSuccessStatusCode();

        //        var stream = await response.Content.ReadAsStreamAsync();

        //        var movies = stream.ReadAndDeserializeFromJson<List<Movie>>();
        //    }
        //}

        private async Task GetMoviesViaMoviesClient(CancellationToken cancellationToken)
        {
            var movies = await _moviesClient.GetMovies(cancellationToken);
        }
    }
}
