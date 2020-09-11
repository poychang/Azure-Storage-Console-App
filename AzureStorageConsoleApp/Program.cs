using AzureStorageTableEntityConverter.Attributes;
using AzureStorageTableRepository;
using System;
using System.Text.Json;

namespace AzureStorageConsoleApp
{
    public class Program
    {
        private static string storageConnectionString = "YOUR_AZURE_STORAGE_CONNECTION_STRING";

        public static void Main(string[] args)
        {
            var repository = new AzureTableRepository<Demo>(new AzureTableRepositoryConfig
            {
                ConnectionString = storageConnectionString
            });

            repository.Create(new Demo { FirstName = "Poy", LastName = "Chang" });
            var data = repository.Read();

            Console.WriteLine(JsonSerializer.Serialize(data));
            Console.ReadKey();
        }
    }

    public class Demo
    {
        [PartitionKey]
        public string LastName { get; set; }

        [RowKey]
        public string FirstName { get; set; }
    }
}
