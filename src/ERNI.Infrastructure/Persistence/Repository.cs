using ERNI.Core;
using ERNI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ERNI.Infrastructure;

internal class Repository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
    private readonly Lazy<ApplicationDbContext> _contextFactory;
    private readonly DbSet<TEntity> _entities;

    public Repository(
        Lazy<ApplicationDbContext> contextFactory,
        DbSet<TEntity> entities)
    {
        _contextFactory = contextFactory;
        _entities = entities;
    }

    public void Add(TEntity entity)
    {
        _entities.Add(entity);
    }

    public ValueTask<TEntity?> FindAsync(params object[] ids)
    {
        return _entities.FindAsync(ids);
    }

    public ValueTask<TEntity?> FindAsync(CancellationToken token, params object[] ids)
    {
        return _entities.FindAsync(ids, token);
    }

    public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
    {
        return _entities.Where(predicate);
    }

    public IQueryable<TEntity> GetAll()
    {
        return _entities.AsQueryable();
    }

    public void Update(TEntity entity)
    {
        _entities.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        _entities.Remove(entity);
    }

    public void Remove(IEnumerable<TEntity> entities)
    {
        _entities.RemoveRange(entities);
    }

    public Task SaveAsync(CancellationToken token)
    {
        return _contextFactory.Value.SaveChangesAsync(token);
    }
}
