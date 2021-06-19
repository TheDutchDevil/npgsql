using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

// TODO: Docs
// The public API analyzer is disabled since these APIs should make it into net6.0, which is what it checks.
#pragma warning disable 1591,RS0016

// ReSharper disable once CheckNamespace
namespace System.Data.Common
{
    public abstract class DbBatch : IDisposable, IAsyncDisposable
    {
        public DbBatchCommandCollection BatchCommands => DbBatchCommands;
        protected abstract DbBatchCommandCollection DbBatchCommands { get; }

        public DbDataReader ExecuteReader()
            => ExecuteDbDataReader();

        protected abstract DbDataReader ExecuteDbDataReader();

        public Task<DbDataReader> ExecuteReaderAsync(CancellationToken cancellationToken = default)
            => ExecuteDbDataReaderAsync(cancellationToken);

        protected abstract Task<DbDataReader> ExecuteDbDataReaderAsync(CancellationToken cancellationToken);

        public abstract int ExecuteNonQuery();

        public abstract Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken = default);

        public abstract object? ExecuteScalar();

        public abstract Task<object?> ExecuteScalarAsync(CancellationToken cancellationToken = default);

        public abstract int Timeout { get; set; }

        public DbConnection? Connection { get; set; }

        protected abstract DbConnection? DbConnection { get; set; }

        public DbTransaction? Transaction { get; set; }
        protected abstract DbTransaction? DbTransaction { get; set; }

        public abstract void Prepare();
        public abstract Task PrepareAsync(CancellationToken cancellationToken = default);
        public abstract void Cancel();

        public void Dispose() { }
        protected virtual void Dispose(bool disposing) {}
        public ValueTask DisposeAsync() => default;
    }

    public abstract class DbBatchCommand
    {
        [DefaultValue("")]
        [RefreshProperties(RefreshProperties.All)]
        [AllowNull]
        public abstract string CommandText { get; set; }

        [DefaultValue(CommandType.Text)]
        [RefreshProperties(RefreshProperties.All)]
        public abstract CommandType CommandType { get; set; }

        [DefaultValue(CommandBehavior.Default)]
        [RefreshProperties(RefreshProperties.All)]
        public abstract CommandBehavior CommandBehavior { get; set; }

        public abstract int RecordsAffected { get; set; }

        public DbParameterCollection Parameters => DbParameterCollection;

        protected abstract DbParameterCollection DbParameterCollection { get; }
    }

    // TODO: Collection? List? Need virtual to allow overriding implementation? Important point...
    public abstract class DbBatchCommandCollection : IList<DbBatchCommand>
    {
        public abstract IEnumerator<DbBatchCommand> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public abstract void Add(DbBatchCommand item);

        public abstract void Clear();

        public abstract bool Contains(DbBatchCommand item);

        public abstract void CopyTo(DbBatchCommand[] array, int arrayIndex);

        public abstract bool Remove(DbBatchCommand item);

        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }

        public abstract int IndexOf(DbBatchCommand item);

        public abstract void Insert(int index, DbBatchCommand item);

        public abstract void RemoveAt(int index);

        public DbBatchCommand this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
