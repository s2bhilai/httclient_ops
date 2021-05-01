using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class StreamService : IIntegrationService
    {
        //private static HttpClient httpClient = new HttpClient();

        private static HttpClient httpClient = new HttpClient(
            new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip
            });

        public StreamService()
        {
            httpClient.BaseAddress = new Uri("http://localhost:57863");
            httpClient.Timeout = new TimeSpan(0, 0, 30);
            httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            //await GetPosterWithStream();

            //await GetPosterWithStreamAndCompletionMode();

            //await PostPosterWithStream();

            await PostAndReadPosterWithStream();
        } 
        
        private async Task GetPosterWithStream()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/movies/6e87f657-f2c1-4d90-9b37-cbe43cc6adb9/posters/{Guid.NewGuid()}");

            request.Headers.Accept.Add
                (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();

                var poster = stream.ReadAndDeserializeFromJson<Poster>();

                //using(var streamReader = new StreamReader(stream))
                //{
                //    using(var jsonTextReader = new JsonTextReader(streamReader))
                //    {
                //        var jsonSerializer = new JsonSerializer();
                //        var poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);
                //    }
                //}
            }
        }

        private async Task GetPosterWithStreamAndCompletionMode()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/movies/6e87f657-f2c1-4d90-9b37-cbe43cc6adb9/posters/{Guid.NewGuid()}");

            request.Headers.Accept.Add
                (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var poster = stream.ReadAndDeserializeFromJson<Poster>();


                //using (var streamReader = new StreamReader(stream))
                //{
                //    using (var jsonTextReader = new JsonTextReader(streamReader))
                //    {
                //        var jsonSerializer = new JsonSerializer();
                //        var poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);
                //    }
                //}
            }
        }

        private async Task PostPosterWithStream()
        {
            //generate a movie poster
            var random = new Random();
            var generatedBytes = new byte[23452];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation()
            {
                Name = "A New POster",
                Bytes = generatedBytes
            };

            //JsonConvert.SerializeObject(posterForCreation); Avoiding this line with streams
            var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeToJsonAndWrite(posterForCreation);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using(var request = new HttpRequestMessage(HttpMethod.Post,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters"))
            {
                request.Headers.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                using(var streamContent = new StreamContent(memoryContentStream))
                {
                    request.Content = streamContent;
                    request.Content.Headers.ContentType = 
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var createdContent = await response.Content.ReadAsStringAsync();
                    var createdPoster = JsonConvert.DeserializeObject<Poster>(createdContent);
                }
            }
        }

        private async Task PostAndReadPosterWithStream()
        {
            //generate a movie poster
            var random = new Random();
            var generatedBytes = new byte[23452];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation()
            {
                Name = "A New POster",
                Bytes = generatedBytes
            };

            //JsonConvert.SerializeObject(posterForCreation); Avoiding this line with streams
            var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeToJsonAndWrite(posterForCreation);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using (var request = new HttpRequestMessage(HttpMethod.Post,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters"))
            {
                request.Headers.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                using (var streamContent = new StreamContent(memoryContentStream))
                {
                    request.Content = streamContent;
                    request.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    using (var response = await httpClient.SendAsync(request,
                        HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        var stream = await response.Content.ReadAsStreamAsync();
                        var poster = stream.ReadAndDeserializeFromJson<Poster>();
                    }
                }
            }
        }

        private async Task GetPosterWithGZipCompression()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/movies/6e87f657-f2c1-4d90-9b37-cbe43cc6adb9/posters/{Guid.NewGuid()}");

            request.Headers.Accept.Add
                (new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add
                (new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();

                var poster = stream.ReadAndDeserializeFromJson<Poster>();
            }
        }
    }
}
