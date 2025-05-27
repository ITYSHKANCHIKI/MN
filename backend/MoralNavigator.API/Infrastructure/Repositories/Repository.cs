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
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _ctx;
        public Repository(AppDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(T entity) => await _ctx.Set<T>().AddAsync(entity);

        public void Delete(T entity) => _ctx.Set<T>().Remove(entity);

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
            => _ctx.Set<T>().Where(predicate).ToList();

        public async Task<IEnumerable<T>> GetAllAsync(
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _ctx.Set<T>();
            if (include != null) query = include(query);
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(
            int id,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _ctx.Set<T>();
            if (include != null) query = include(query);
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
            => await _ctx.Set<T>().SingleOrDefaultAsync(predicate);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
            => await _ctx.Set<T>().AnyAsync(predicate);

        public void Update(T entity) => _ctx.Set<T>().Update(entity);
    }
}
