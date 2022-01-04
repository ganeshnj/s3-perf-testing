using Amazon.Runtime;
using System;
using System.Net.Http;
using System.Threading;

namespace S3PerfTest
{
    public class CustomHttpClientFactory : HttpClientFactory
    {
        static HttpClient httpClient = null;
        static CustomHttpClientFactory()
        {
            var socketsHandler = new SocketsHttpHandler
            {
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
                KeepAlivePingDelay = TimeSpan.FromSeconds(2),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(6)
            };
            httpClient = new HttpClient(socketsHandler);
            httpClient.DefaultRequestHeaders.ConnectionClose = false;
        }

        public override HttpClient CreateHttpClient(IClientConfig clientConfig)
        {
            return httpClient;
        }
    }
}
