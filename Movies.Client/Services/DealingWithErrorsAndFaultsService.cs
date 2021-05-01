using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class DealingWithErrorsAndFaultsService : IIntegrationService
    {
        private IHttpClientFactory _httpClientFactory;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public DealingWithErrorsAndFaultsService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task Run()
        {
            //await GetMovieAndDealWithInvalidResponse(cancellationTokenSource.Token);

            await PostMovieAndHandlevalidationErrors(cancellationTokenSource.Token);
        }

        private async Task GetMovieAndDealWithInvalidResponse(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MoviesClient");

            var request = new HttpRequestMessage(HttpMethod.Get,
                "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee1234");

            request.Headers.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.AcceptEncoding
                .Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            using(var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,cancellationToken))
            {
                if(!response.IsSuccessStatusCode)
                {
                    //inspect the status code
                    if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        //Show this to user
                        Console.WriteLine("The requested movie cannot be found");
                    }
                    else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        //trigger login flow
                        return;
                    }
                    response.EnsureSuccessStatusCode(); //throws exception
                }

                var stream = await response.Content.ReadAsStreamAsync();

                var movie = stream.ReadAndDeserializeFromJson<Movie>();
            }

        }

        private async Task PostMovieAndHandlevalidationErrors(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MoviesClient");

            var movieToCreate = new MovieForCreation()
            {
                Title = "Reservior Dogs III",
                Description = "too short",
                DirectorId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var serializedMovieForCreation = JsonConvert.SerializeObject(movieToCreate);

            using(var request = new HttpRequestMessage(HttpMethod.Post,"api/movies"))
            {
                request.Headers.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                request.Headers.AcceptEncoding
                    .Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

                request.Content = new StringContent(serializedMovieForCreation);
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                using(var response = await httpClient
                    .SendAsync(request,HttpCompletionOption.ResponseHeadersRead,cancellationToken))
                {
                    if(!response.IsSuccessStatusCode)
                    {
                        if(response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                        {
                            var errorStream = await response.Content.ReadAsStreamAsync();

                            using (var streamReader = new StreamReader(errorStream))
                            {
                                using (var jsonTextReader = new JsonTextReader(streamReader))
                                {
                                    var jsonSerializer = new JsonSerializer();
                                    var validationErrors = jsonSerializer.Deserialize(jsonTextReader);
                                    Console.WriteLine(validationErrors);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            response.EnsureSuccessStatusCode();
                        }
                    }

                    var stream = await response.Content.ReadAsStreamAsync();
                    var movie = stream.ReadAndDeserializeFromJson<Movie>();
                }
            }
        }
    }
}
