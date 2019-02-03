using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace AddPartitionKey
{
    public class Entity : TableEntity
    {
        public Entity()
        {

        }

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

    public static class Helper
    {
        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
