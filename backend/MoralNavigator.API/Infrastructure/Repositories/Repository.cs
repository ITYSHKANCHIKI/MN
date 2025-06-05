// File: backend/MoralNavigator.API/Infrastructure/Repositories/Repository.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoralNavigator.API.Domain.Interfaces;
using MoralNavigator.API.Infrastructure.Data;

namespace MoralNavigator.API.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly AppDbContext _ctx;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(AppDbContext context)
        {
            _ctx = context;
            _dbSet = _ctx.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? include = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (include != null)
                query = include(query);

            // Предполагаем, что у всех сущностей есть поле Id типа int
            return await query.FirstOrDefaultAsync(e =>
                EF.Property<int>(e, "Id") == id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
