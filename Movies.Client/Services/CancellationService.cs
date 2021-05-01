using Movies.Client.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Movies.Client;
using System.Threading;

namespace Movies.Client.Services
{
    public class CancellationService : IIntegrationService
    {
        private static HttpClient httpClient = new HttpClient(
            new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
            });

        private CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        public CancellationService()
        {
            httpClient.BaseAddress = new Uri("http://localhost:57863");
            httpClient.Timeout = new TimeSpan(0, 0, 30);
            httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            //_cancellationTokenSource.CancelAfter(2000);
            //await GetTrailerAndCancel(_cancellationTokenSource.Token);
            await GetTrailerAndHandleTimeout();
        }

        private async Task GetTrailerAndCancel(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

            request.Headers.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            try
            {
                using (var response = await httpClient
                .SendAsync(request,
                           HttpCompletionOption.ResponseHeadersRead,
                           cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();

                    var trailer = stream.ReadAndDeserializeFromJson<Trailer>();
                }
            }
            catch(OperationCanceledException ex)
            {
                Console.WriteLine($"An operation was cancelled with message {ex.Message}");
                //additional cleanup
            }
        }

        private async Task GetTrailerAndHandleTimeout()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

            request.Headers.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            try
            {
                using (var response = await httpClient
                .SendAsync(request,
                           HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var stream = await response.Content.ReadAsStreamAsync();

                    var trailer = stream.ReadAndDeserializeFromJson<Trailer>();
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"An operation was cancelled with message {ex.Message}");
                //additional cleanup
            }
        }
    }
}
