using System;

namespace AzureStorageTableEntityConverter.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PartitionKeyAttribute : Attribute
    {
    }
}
