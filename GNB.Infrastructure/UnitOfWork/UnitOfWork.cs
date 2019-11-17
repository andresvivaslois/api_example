using GNB.Infrastructure.UnitOfWork.Interfaces;
using GNB.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Concurrent;

namespace GNB.Infrastructure.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork, IDisposable where TContext: DbContext
    {
        //Props
        private readonly ConcurrentDictionary<Type, object> _repositories;
        private IDesignTimeDbContextFactory<DbContext> ContextFactory { get; set; }
        private DbContext _context;
        public DbContext Context
        {
            get {
                if (_context == null)
                {
                    _context = this.ContextFactory.CreateDbContext(new[] { string.Empty });
                }
                return _context;
            }
        }
        private bool IsDisposed { get; set; }

        //Constructor
        public UnitOfWork(IDesignTimeDbContextFactory<DbContext> factory)
        {
            ContextFactory = factory;
            _repositories = new ConcurrentDictionary<Type, object>();
        }

        //Methods
        public bool CheckDBExists()
        {
            return this.Context.Database.EnsureCreated();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return (Repository<TEntity>)_repositories.GetOrAdd(typeof(TEntity), new Repository<TEntity>(this.Context));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.IsDisposed && disposing && _context != null)
            {
                _context.Dispose();
            }
            this.IsDisposed = true;
        }

        public virtual bool Save()
        {
            try
            {
                return 0 < this.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
                return false;
            }
        }

    }
}
