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
            var socketsHandler = new SocketsHttpHandler();
            httpClient = new HttpClient(socketsHandler);
            httpClient.DefaultRequestHeaders.ConnectionClose = false;
            httpClient.DefaultRequestHeaders.Connection.Add("Keep-Alive");
            httpClient.DefaultRequestHeaders.Add("Keep-Alive", "timeout=10, max=1000");
        }

        public override HttpClient CreateHttpClient(IClientConfig clientConfig)
        {
            return httpClient;
        }
    }
}
