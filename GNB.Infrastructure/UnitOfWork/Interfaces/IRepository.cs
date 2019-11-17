using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GNB.Infrastructure.UnitOfWork.Interfaces
{
    public interface IRepository<TEntity>
    {
        IEnumerable<TEntity> FindAll();
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> where);
        Task<bool> DeleteAndInsert(List<TEntity> lEntities);
    }
}
