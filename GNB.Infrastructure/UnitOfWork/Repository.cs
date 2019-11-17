using GNB.Infrastructure.UnitOfWork.Interfaces;
using GNB.Utilities.Helpers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GNB.Infrastructure.UnitOfWork
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbContext Context { get; set; }

        public Repository(DbContext context)
        {
            this.Context = context;
        }

        public IEnumerable<TEntity> FindAll()
        {
            try
            {
                IQueryable<TEntity> query = null;

                query = this.Context.Set<TEntity>();

                return query.ToList();

            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
                return null;
            }
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                if (where != null)
                {
                    return this.Context.Set<TEntity>().Where(where);
                }
                else
                {
                    return FindAll();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
                return null;
            }

        }


        public async Task<bool> DeleteAndInsert(List<TEntity> lEntities)
        {
            bool result = true;

            if (lEntities != null && lEntities.Any())
            {
                var firstElement = lEntities.First();
                var table = firstElement.GetType().Name;

                using (var transaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        var query = $"delete from {table}";

                        await Context.Database.ExecuteSqlCommandAsync(query);

                        Context.AddRange(lEntities);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.LogError(ex.ToString());
                        transaction.Rollback();
                        result = false;
                    }
                }
                return result;
            }

            return false;


        }

    }
}
