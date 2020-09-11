using AzureStorageTableEntityConverter.Attributes;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;

namespace AzureStorageTableEntityConverter.Tests
{
    [TestClass]
    public class TableEntityConverterUnitTest
    {
        private Customer NewCustomer { get; set; }

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void ToTableEntity_ConvertObjectToTableEntity()
        {
            var customer = new Customer
            {
                LastName = "Smith",
                FirstName = "Jeff",
            };
            var tableEntity = EntityConverter.ToTableEntity(customer);
            var failMessage = $"{typeof(ITableEntity)}, {tableEntity.GetType()}";

            Assert.IsTrue(tableEntity is ITableEntity, failMessage);
        }

        [TestMethod]
        public void ToObject()
        {
            var customer = new Customer
            {
                LastName = "Smith",
                FirstName = "Jeff",
            };
            var tableEntity = EntityConverter.ToTableEntity(customer);
            var obj = EntityConverter.ToObject<Customer>(tableEntity);
            var failMessage = $"{JsonSerializer.Serialize(customer)}\r\n{JsonSerializer.Serialize(obj)}";

            Assert.AreEqual(JsonSerializer.Serialize(customer), JsonSerializer.Serialize(obj), failMessage);
        }

        public class Customer
        {
            // PartitionKey must be of type string and exist exactly one time per class
            [PartitionKey]
            public string LastName { get; set; }

            // PartitionKey must be of type string and exist exactly one time per class
            [RowKey]
            public string FirstName { get; set; }
        }
    }
}
