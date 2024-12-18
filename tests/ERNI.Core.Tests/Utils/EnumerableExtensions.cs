using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace ERNI.Core.Tests;

// Disclaimer author, https://stackoverflow.com/a/63715931
public static class EnumerableExtensions
{
    public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source)
    {
        return new AsyncQueryable<T>(source);
    }

    private class AsyncQueryable<TEntity> : EnumerableQuery<TEntity>, IAsyncEnumerable<TEntity>, IQueryable<TEntity>
    {
        public AsyncQueryable(IEnumerable<TEntity> enumerable)
            : base(enumerable)
        {
        }

        public AsyncQueryable(Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<TEntity> GetEnumerator() => new AsyncEnumerator(this.AsEnumerable().GetEnumerator());

        public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
            new AsyncEnumerator(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new AsyncQueryProvider(this);

        private class AsyncEnumerator(IEnumerator<TEntity> inner) : IAsyncEnumerator<TEntity>
        {
            private readonly IEnumerator<TEntity> _inner = inner;

            public TEntity Current => _inner.Current;

            public void Dispose() => _inner.Dispose();

            public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(_inner.MoveNext());

            public async ValueTask DisposeAsync() => _inner.Dispose();
        }

        private class AsyncQueryProvider : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal AsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public static IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new AsyncQueryable<TResult>(expression);

            public IQueryable CreateQuery(Expression expression) => new AsyncQueryable<TEntity>(expression);

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new AsyncQueryable<TElement>(expression);

            public object? Execute(Expression expression) => _inner.Execute(expression);

            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

            TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Execute<TResult>(expression);
        }
    }
}
