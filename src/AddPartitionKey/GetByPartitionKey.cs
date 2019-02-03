using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.Threading.Tasks;

namespace AddPartitionKey
{
    public static class GetByPartitionKey
    {
        [FunctionName("GetByPartitionKey")]
        public async static Task Run([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var cloudStorage = CloudStorageAccount.Parse(Helper.GetEnvironmentVariable("AzureWebJobsStorage"));
            var cloudTableClient = cloudStorage.CreateCloudTableClient();
            var dummyClient = cloudTableClient.GetTableReference("dummy");

            //A random guid 
            var pk = Guid.NewGuid().ToString();

            var queryOp = new TableQuery<Entity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, pk));

            var results = await dummyClient.ExecuteQuerySegmentedAsync<Entity>(queryOp, null);

            log.LogInformation($"Number of rows retrieved {results.Results.Count}");
        }
    }
}
