using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Npgsql
{
    public class NpgsqlBatchCommandCollection : DbBatchCommandCollection
    {
        readonly List<NpgsqlBatchCommand> _list;

        internal NpgsqlBatchCommandCollection(List<NpgsqlBatchCommand> batchCommands)
            => _list = batchCommands;

        public override IEnumerator<DbBatchCommand> GetEnumerator() => _list.GetEnumerator();

        public override void Add(DbBatchCommand item)
            => _list.Add(Cast(item));

        public override void Clear() => _list.Clear();

        public override bool Contains(DbBatchCommand item) => _list.Contains(Cast(item));

        public override void CopyTo(DbBatchCommand[] array, int arrayIndex) => throw new NotImplementedException();

        public override bool Remove(DbBatchCommand item) => _list.Remove(Cast(item));

        public override int Count => _list.Count;
        public override bool IsReadOnly => false;

        public override int IndexOf(DbBatchCommand item) => _list.IndexOf(Cast(item));

        public override void Insert(int index, DbBatchCommand item) => _list.Insert(index, Cast(item));

        public override void RemoveAt(int index) => _list.RemoveAt(index);

        static NpgsqlBatchCommand Cast(object? value)
            => value is NpgsqlBatchCommand c
                ? c
                : throw new InvalidCastException(
                    $"The value \"{value}\" is not of type \"{nameof(NpgsqlBatchCommand)}\" and cannot be used in this batch command collection.");
    }
}
