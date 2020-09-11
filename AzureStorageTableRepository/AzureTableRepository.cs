using AzureStorageTableEntityConverter;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AzureStorageTableRepository
{
    public class AzureTableRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private CloudTable _cloudTable { get; set; }

        /// <summary>建構式</summary>
        /// <param name="config">設定檔</param>
        public AzureTableRepository(AzureTableRepositoryConfig config)
        {
            var cloudStorageAccount = config.IsDevelopment
                ? CloudStorageAccount.DevelopmentStorageAccount
                : CloudStorageAccount.Parse(config.ConnectionString);

            _cloudTable = cloudStorageAccount
                .CreateCloudTableClient(new TableClientConfiguration())
                .GetTableReference(typeof(TEntity).Name);
            _cloudTable.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public void Create(TEntity entity)
        {
            Update(entity);
        }

        public IQueryable<TEntity> Read()
        {
            var query = new TableQuery<DynamicTableEntity>();

            return _cloudTable.ExecuteQuery(query)
                .Select(tableEntity => EntityConverter.ToObject<TEntity>(tableEntity))
                .AsQueryable();
        }

        public void Update(TEntity entity)
        {
            var tableEntity = EntityConverter.ToTableEntity(entity);
            var operation = TableOperation.InsertOrMerge(tableEntity);

            _cloudTable.ExecuteAsync(operation).GetAwaiter().GetResult();
        }

        public void Delete(TEntity entity)
        {
            var tableEntity = EntityConverter.ToTableEntity(entity);
            var operation = TableOperation.Delete(tableEntity);

            _cloudTable.Execute(operation);
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
