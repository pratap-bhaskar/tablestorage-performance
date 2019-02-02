using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AddPartitionKey
{
    public static class Function1
    {
        private static Random rand = new Random();
        [FunctionName("Function1")]
        public static async Task Run([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            var cloudStorage = CloudStorageAccount.Parse(GetEnvironmentVariable("AzureWebJobsStorage"));
            var cloudTableClient = cloudStorage.CreateCloudTableClient();
            var timeClient = cloudTableClient.GetTableReference("time");
            var dummyClient = cloudTableClient.GetTableReference("dummy");

            await timeClient.CreateIfNotExistsAsync();
            await dummyClient.CreateIfNotExistsAsync();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            var pk = Guid.NewGuid().ToString();
            var random = rand.Next(10, 30);

            var insertOp = new TableBatchOperation();
            
            for (int i = 0; i < random ; i++)
            {
                insertOp.Insert(new Entity(pk));
            }
            await dummyClient.ExecuteBatchAsync(insertOp);

            watch.Stop();

            await timeClient.ExecuteAsync(TableOperation.Insert(new TimeTaken(pk) { TimeInMs = watch.ElapsedMilliseconds }));
        }

        public class Entity : TableEntity
        {
            public Entity(string partitionKey)
            {
                PartitionKey = partitionKey;
                RowKey = Guid.NewGuid().ToString();
            }
        }

        public class TimeTaken : TableEntity
        {
            public long TimeInMs { get; set; }

            public TimeTaken(string partitionKey)
            {
                PartitionKey = partitionKey;
                RowKey = string.Empty;
            }
        }

        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
