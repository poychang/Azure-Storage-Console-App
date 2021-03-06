﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace AzureStorageTableRepository
{
    /// <summary>Entity Repository 介面</summary>
    /// <typeparam name="TEntity">任意 Entity</typeparam>
    public interface IRepository<TEntity>
    {
        /// <summary>新增一筆資料</summary>
        /// <param name="entity">要新增的 Entity</param>
        void Create(TEntity entity);

        /// <summary>取得全部資料</summary>
        /// <returns>可查詢的 Entity 資料</returns>
        IQueryable<TEntity> Read();

        /// <summary>更新一筆資料</summary>
        /// <param name="entity">要被更新的 Entity</param>
        void Update(TEntity entity);

        /// <summary>刪除一筆資料</summary>
        /// <param name="entity">要被刪除的 Entity</param>
        void Delete(TEntity entity);

        /// <summary>儲存異動</summary>
        void SaveChanges();
    }
}
