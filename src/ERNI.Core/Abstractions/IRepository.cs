using ERNI.Entities;
using System.Linq.Expressions;

namespace ERNI.Core;

public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    void Add(TEntity entity);

    ValueTask<TEntity?> FindAsync(params object[] ids);

    ValueTask<TEntity?> FindAsync(CancellationToken token, params object[] ids);

    IQueryable<TEntity> GetAll();

    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    void Remove(IEnumerable<TEntity> entities);

    Task SaveAsync(CancellationToken token);
}
