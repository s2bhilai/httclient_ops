using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class HttpHandlersService : IIntegrationService
    {
        private IHttpClientFactory _httpClientFactory;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public HttpHandlersService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task Run()
        {
            await GetMoviesWithRetryPolicy(cancellationTokenSource.Token);
        }
        
        private async Task GetMoviesWithRetryPolicy(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MoviesClient");

            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee12");
            request.Headers.Accept.Add
                (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add
                (new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            using(var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,cancellationToken))
            {
                if(!response.IsSuccessStatusCode)
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("The Requested Movie not found");
                        return;
                    }

                    response.EnsureSuccessStatusCode();
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var movie = stream.ReadAndDeserializeFromJson<Movie>();
            }

        }
    }
}
