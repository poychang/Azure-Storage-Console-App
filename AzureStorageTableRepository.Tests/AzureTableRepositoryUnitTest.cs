using AzureStorageTableEntityConverter.Attributes;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.Json;

namespace AzureStorageTableRepository.Tests
{
    [TestClass]
    public class AzureTableRepositoryUnitTest
    {
        private AzureTableRepository<Customer> Repository { get; set; }
        private Customer NewCustomer { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            // Before running this unit test, you have to execute Azure Storage Emulator first.

            Repository = new AzureTableRepository<Customer>(new AzureTableRepositoryConfig { IsDevelopment = true });
            NewCustomer = new Customer
            {
                FirstName = "Poy",
                LastName = "Chang",
                Email = "old@poychang.net",
                TemporaryLoginHash = Guid.NewGuid().ToString("N"),
                Address = new Address
                {
                    Street = "Back Street",
                    City = "Taipei"
                }
            };
        }

        [TestMethod]
        public void Create_ThenAzureStorageTableHasData()
        {
            Repository.Create(NewCustomer);
            var data = Repository.Read();
            var result = data.Any(data => data.LastName == NewCustomer.LastName && data.FirstName == NewCustomer.FirstName);
            var failMessage = JsonSerializer.Serialize(data);

            Assert.IsTrue(result, failMessage);
        }

        [TestMethod]
        public void Read_ThenReturnData()
        {
            Repository.Create(NewCustomer);
            var data = Repository.Read();
            var result = data.Any();
            var failMessage = JsonSerializer.Serialize(data);

            Assert.IsTrue(result, failMessage);
        }

        [TestMethod]
        public void Update_ThenDataIsChanged()
        {
            var changedValue = "new@poychang.net";
            NewCustomer.Email = changedValue;
            Repository.Update(NewCustomer);
            var data = Repository.Read();
            var result = data.First(data => data.LastName == NewCustomer.LastName && data.FirstName == NewCustomer.FirstName);
            var failMessage = JsonSerializer.Serialize(data);

            Assert.AreEqual(changedValue, result.Email, failMessage);
        }

        [TestMethod]
        public void Delete_ThenDataIsDelete()
        {
            Repository.Delete(NewCustomer);
            var data = Repository.Read();
            var result = data.Any(data => data.LastName == NewCustomer.LastName && data.FirstName == NewCustomer.FirstName);
            var failMessage = JsonSerializer.Serialize(data);

            Assert.IsFalse(result, failMessage);
        }
    }

    public class Customer
    {
        // PartitionKey must be of type string and exist exactly one time per class
        [PartitionKey]
        public string LastName { get; set; }

        // PartitionKey must be of type string and exist exactly one time per class
        [RowKey]
        public string FirstName { get; set; }

        // ETag is optional and can safely have a private setter
        [ETag]
        public string ETag { get; private set; }

        // Timestamp is optional and can safely have a private setter
        [Timestamp]
        public DateTimeOffset Timestamp { get; private set; }

        // Ordinary string field will be stored as string in the entity
        public string Email { get; set; }

        // Complex type will be converted to JSON and stored as string
        public Address Address { get; set; }

        // IgnoreProperty marks a property to be ignored when creating the TableEntity
        [IgnoreProperty]
        public string TemporaryLoginHash { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }
    }
}
