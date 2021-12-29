using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace S3PerfTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new AmazonS3Client();
            var logs = new List<Log>();

            var path = Path.Combine("..", "..", "..", "Results", $"{DateTime.UtcNow.ToString("yyyy-MM-dd-THH-mm-ss")}.csv");
            await File.AppendAllLinesAsync(path, new[] { Log.Header });


            for (int i = 0; i < int.MaxValue; i++)
            {
                var log = new Log();
                using (var listener = new ConsoleEventListener(log))
                {
                    log.SdkRequestStart = DateTime.UtcNow;

                    var obj = await client.GetObjectAsync(new GetObjectRequest
                    {
                        BucketName = "jangirg-s3-perf-test",
                        Key = "test.json"
                    });

                    log.SdkRequestEnd = DateTime.UtcNow;
                    log.ResponseReadStart = DateTime.UtcNow;

                    using (var reader = new StreamReader(obj.ResponseStream))
                    {
                        var time = DateTime.UtcNow;
                        reader.ReadToEnd();
                        var elapsed = DateTime.UtcNow - time;
                    }
                    log.ResponseReadEnd = DateTime.UtcNow;
                    logs.Add(log);
                }
                await File.AppendAllLinesAsync(path, new [] {log.ToString()});
            }
        }
    }
}
