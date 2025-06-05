// File: backend/MoralNavigator.API/Domain/Interfaces/IRepository.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MoralNavigator.API.Domain.Interfaces
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);

        Task<TEntity?> GetByIdAsync(int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null);

        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);

        // При необходимости добавьте сюда Update, Find и т. д.
    }
}
