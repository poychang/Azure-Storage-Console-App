using AzureStorageTableEntityConverter.Converters;
using Microsoft.Azure.Cosmos.Table;
using System;

namespace AzureStorageTableEntityConverter
{
    public static class EntityConverter
    {
        public static ITableEntity ToTableEntity(object poco)
        {
            return new ObjectToTableEntityConverter(poco).GetTableEntity();
        }

        public static T ToObject<T>(DynamicTableEntity tableEntity) where T : class, new()
        {
            return new TableEntityToObjectConverter<T>(tableEntity).GetObject();
        }

        public static T ToObject<T>(object tableEntity) where T : class, new()
        {
            var dynamicTableEntity = tableEntity as DynamicTableEntity;

            if (dynamicTableEntity == default(DynamicTableEntity))
            {
                throw new ArgumentException("Parameter has to be of type DynamicTableEntity", nameof(tableEntity));
            }

            return ToObject<T>(dynamicTableEntity);
        }
    }
}
