using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace S3PerfTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var config = new AmazonS3Config();
            config.HttpClientFactory = new CustomHttpClientFactory();
            var client = new AmazonS3Client(config);
            var logs = new List<Log>();

            var path = Path.Combine("..", "..", "..", "Results", $"{DateTime.UtcNow.ToString("yyyy-MM-dd-THH-mm-ss")}.csv");
            await File.AppendAllLinesAsync(path, new[] { Log.Header });

            var httpClient = new HttpClient();


            for (int i = 0; i < int.MaxValue; i++)
            {
                var log = new Log();
                using (var listener = new ConsoleEventListener(log))
                {
                    log.SignStart = DateTime.UtcNow;

                    var url = client.GetPreSignedURL(new GetPreSignedUrlRequest
                    {
                        BucketName = "jangirg-s3-perf-test",
                        Key = "test.json",
                        Expires = DateTime.UtcNow.AddHours(1)
                    });

                    log.SignStop = DateTime.UtcNow;

                    log.GetStart = DateTime.UtcNow;
                    var response = await httpClient.GetAsync(url);
                    log.GetStop = DateTime.UtcNow;

                    Debug.Assert(response.StatusCode == System.Net.HttpStatusCode.OK);

                    log.ResponseReadStart = DateTime.UtcNow;

                    using (var reader = new StreamReader(response.Content.ReadAsStream()))
                    {
                        var time = DateTime.UtcNow;
                        var content = reader.ReadToEnd();
                        var elapsed = DateTime.UtcNow - time;
                    }
                    log.ResponseReadEnd = DateTime.UtcNow;
                    logs.Add(log);
                }
                await File.AppendAllLinesAsync(path, new[] { log.ToString() });

                //await Task.Delay(TimeSpan.FromSeconds(7));
            }
        }
    }
}
