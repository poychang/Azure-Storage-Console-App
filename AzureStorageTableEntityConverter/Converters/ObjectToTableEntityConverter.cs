using AzureStorageTableEntityConverter.Attributes;
using AzureStorageTableEntityConverter.Extensions;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AzureStorageTableEntityConverter.Converters
{
    public class ObjectToTableEntityConverter
    {
        private object _sourceObject;
        private PropertyInfo[] _reflectedProperties;
        private Type[] _ignoredPropertyAttributeTypes = new[] {
            typeof(IgnorePropertyAttribute),
            typeof(ETagAttribute),
            typeof(PartitionKeyAttribute),
            typeof(RowKeyAttribute),
            typeof(TimestampAttribute)
        };

        public ObjectToTableEntityConverter(object value)
        {
            _sourceObject = value;
            _reflectedProperties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        private string GetPartitionKey()
        {
            var property = _reflectedProperties.Single(typeof(PartitionKeyAttribute));
            return (string)property.GetValue(_sourceObject);
        }

        private string GetRowKey()
        {
            var property = _reflectedProperties.Single(typeof(RowKeyAttribute));
            return (string)property.GetValue(_sourceObject);
        }

        private string GetETag()
        {
            var property = _reflectedProperties.SingleOrDefault(typeof(ETagAttribute));
            //return property != default(PropertyInfo) ? (string)property.GetValue(_sourceObject) : default(string);
            return "*";
        }

        private IDictionary<string, EntityProperty> GetEntityProperties()
        {
            var result = new Dictionary<string, EntityProperty>();

            foreach (var property in _reflectedProperties)
            {
                if (!IsIgnoredProperty(property))
                {
                    var value = property.GetValueAsEntityProperty(_sourceObject);
                    result.Add(property.Name, value);
                }
            }

            return result;
        }

        public ITableEntity GetTableEntity()
        {
            return new DynamicTableEntity
            {
                PartitionKey = GetPartitionKey(),
                RowKey = GetRowKey(),
                ETag = GetETag(),
                Properties = GetEntityProperties()
            };
        }

        private bool IsIgnoredProperty(PropertyInfo property)
        {
            var attributeTypes = property.CustomAttributes.Select(attribute => attribute.AttributeType);

            foreach (var attributeType in attributeTypes)
            {
                if (_ignoredPropertyAttributeTypes.Any(ignoredType => ignoredType.IsEquivalentTo(attributeType)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
