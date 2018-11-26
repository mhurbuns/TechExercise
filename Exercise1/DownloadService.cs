using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Exercise1
{
    public class DownloadService
    {
        private int _length;

        readonly CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _httpClient;

        public DownloadService()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _httpClient = new HttpClient();

        }

        private void UpdateLength(string length)
        {
            _length += int.Parse(length);
        }

        public Task<int> DownLoad(IEnumerable<string> urlList)
        {
            ParallelOptions po = new ParallelOptions
            {
                CancellationToken = _cancellationTokenSource.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            try
            {
                Parallel.ForEach(urlList, po, (url) =>
                {
                    HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                    Console.WriteLine(response.StatusCode);
                    string length = response.Content.Headers.FirstOrDefault(x => x.Key == "Content-Length").Value.Single();
                    Console.WriteLine($"content length {length}");
                    UpdateLength(length);
                    po.CancellationToken.ThrowIfCancellationRequested();
                });
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _cancellationTokenSource.Dispose();
            }

            return Task.FromResult(_length);
        }

        public bool Cancel()
        {
            _cancellationTokenSource.Cancel();
            return true;
        }
    }
}