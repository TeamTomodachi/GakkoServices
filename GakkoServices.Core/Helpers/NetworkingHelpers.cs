using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GakkoServices.Core.Helpers
{
    public static class NetworkingHelpers
    {
        // reuse the http client across requests (it's reentrant and threadsafe and all that)
        private static HttpClient _httpClient = new HttpClient();

        async public static Task<HttpResponseMessage> WaitForOk(Uri path, int delay = 1000, CancellationToken? token = null)
        {
            while (true)
            {
                token?.ThrowIfCancellationRequested();
                try
                {
                    // if the request was successful, return
                    return await _httpClient.GetAsync(path);
                }
                catch (HttpRequestException)
                {
                    // otherwise, wait a bit and try again
                    await Task.Delay(delay);
                }
            }
        }
    }
}