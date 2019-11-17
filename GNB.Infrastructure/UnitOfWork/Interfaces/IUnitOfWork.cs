using System;
using System.Collections.Generic;
using System.Text;

namespace GNB.Infrastructure.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        bool CheckDBExists();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        void Dispose();
    }
}
