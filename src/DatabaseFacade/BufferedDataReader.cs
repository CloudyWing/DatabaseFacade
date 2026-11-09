using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace CloudyWing.DatabaseFacade {
    /// <summary>
    /// Holds one result set read asynchronously from another reader, so it can be loaded with <see cref="DataTable.Load(IDataReader)" />.
    /// </summary>
    /// <remarks>
    /// The schema table, field names, and field types are captured from the source reader,
    /// so <see cref="DataTable.Load(IDataReader)" /> builds the same columns and constraints as it does with the source reader.
    /// </remarks>
    internal sealed class BufferedDataReader : DbDataReader {
        private readonly DataTable? schemaTable;
        private readonly string[] names;
        private readonly Type[] fieldTypes;
        private readonly string[] dataTypeNames;
        private readonly List<object[]> rows;
        private readonly int visibleFieldCount;
        private readonly int recordsAffected;
        private int rowIndex = -1;
        private bool isClosed;

        private BufferedDataReader(
            DataTable? schemaTable,
            string[] names,
            Type[] fieldTypes,
            string[] dataTypeNames,
            List<object[]> rows,
            int visibleFieldCount,
            int recordsAffected
        ) {
            this.schemaTable = schemaTable;
            this.names = names;
            this.fieldTypes = fieldTypes;
            this.dataTypeNames = dataTypeNames;
            this.rows = rows;
            this.visibleFieldCount = visibleFieldCount;
            this.recordsAffected = recordsAffected;
        }

        public override int Depth => 0;

        public override int FieldCount => names.Length;

        public override int VisibleFieldCount => visibleFieldCount;

        public override bool HasRows => rows.Count > 0;

        public override bool IsClosed => isClosed;

        public override int RecordsAffected => recordsAffected;

        private object[] CurrentRow {
            get {
                if (rowIndex < 0 || rowIndex >= rows.Count) {
                    throw new InvalidOperationException("No data exists for the row.");
                }

                return rows[rowIndex];
            }
        }

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        /// <summary>
        /// Reads the current result set of the source reader into memory.
        /// </summary>
        /// <param name="source">The reader positioned at the result set to buffer.</param>
        /// <param name="cancellationToken">The token used to cancel the asynchronous operation.</param>
        /// <returns>A task that returns the buffered reader.</returns>
        public static async Task<BufferedDataReader> LoadAsync(DbDataReader source, CancellationToken cancellationToken) {
            int fieldCount = source.FieldCount;
            // 與 DataTable.Load 相同，在讀取資料列之前取得 Schema，SequentialAccess 下也能使用
            DataTable? schemaTable = source.GetSchemaTable();
            string[] names = new string[fieldCount];
            Type[] fieldTypes = new Type[fieldCount];
            string[] dataTypeNames = new string[fieldCount];

            for (int i = 0; i < fieldCount; i++) {
                names[i] = source.GetName(i);
                fieldTypes[i] = source.GetFieldType(i);
                dataTypeNames[i] = source.GetDataTypeName(i);
            }

            List<object[]> rows = [];
            while (await source.ReadAsync(cancellationToken).ConfigureAwait(false)) {
                object[] values = new object[fieldCount];
                source.GetValues(values);
                rows.Add(values);
            }

            return new BufferedDataReader(
                schemaTable,
                names,
                fieldTypes,
                dataTypeNames,
                rows,
                source.VisibleFieldCount,
                source.RecordsAffected
            );
        }

        public override void Close() {
            isClosed = true;
        }

        public override DataTable? GetSchemaTable() {
            return schemaTable;
        }

        public override bool NextResult() {
            rowIndex = rows.Count;
            return false;
        }

        public override bool Read() {
            if (isClosed || rowIndex + 1 >= rows.Count) {
                rowIndex = rows.Count;
                return false;
            }

            rowIndex++;
            return true;
        }

        public override string GetName(int ordinal) {
            return names[ordinal];
        }

        public override int GetOrdinal(string name) {
            int index = Array.FindIndex(names, x => string.Equals(x, name, StringComparison.Ordinal));
            if (index < 0) {
                index = Array.FindIndex(names, x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
            }

            return index >= 0 ? index : throw new IndexOutOfRangeException($"Column '{name}' was not found.");
        }

        public override Type GetFieldType(int ordinal) {
            return fieldTypes[ordinal];
        }

        public override string GetDataTypeName(int ordinal) {
            return dataTypeNames[ordinal];
        }

        public override object GetValue(int ordinal) {
            return CurrentRow[ordinal];
        }

        public override int GetValues(object[] values) {
            object[] row = CurrentRow;
            int count = Math.Min(values.Length, row.Length);
            Array.Copy(row, values, count);
            return count;
        }

        public override bool IsDBNull(int ordinal) {
            return CurrentRow[ordinal] is DBNull;
        }

        public override bool GetBoolean(int ordinal) => (bool)GetValue(ordinal);

        public override byte GetByte(int ordinal) => (byte)GetValue(ordinal);

        public override char GetChar(int ordinal) => (char)GetValue(ordinal);

        public override DateTime GetDateTime(int ordinal) => (DateTime)GetValue(ordinal);

        public override decimal GetDecimal(int ordinal) => (decimal)GetValue(ordinal);

        public override double GetDouble(int ordinal) => (double)GetValue(ordinal);

        public override float GetFloat(int ordinal) => (float)GetValue(ordinal);

        public override Guid GetGuid(int ordinal) => (Guid)GetValue(ordinal);

        public override short GetInt16(int ordinal) => (short)GetValue(ordinal);

        public override int GetInt32(int ordinal) => (int)GetValue(ordinal);

        public override long GetInt64(int ordinal) => (long)GetValue(ordinal);

        public override string GetString(int ordinal) => (string)GetValue(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) {
            byte[] value = (byte[])GetValue(ordinal);
            return CopyValues(value, dataOffset, buffer, bufferOffset, length);
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) {
            object value = GetValue(ordinal);
            char[] chars = value is string text ? text.ToCharArray() : (char[])value;
            return CopyValues(chars, dataOffset, buffer, bufferOffset, length);
        }

        public override IEnumerator GetEnumerator() {
            return new DbEnumerator(this);
        }

        private static long CopyValues<T>(T[] source, long dataOffset, T[]? buffer, int bufferOffset, int length) {
            if (buffer is null) {
                return source.Length;
            }

            int count = (int)Math.Max(0, Math.Min(length, source.Length - dataOffset));
            Array.Copy(source, dataOffset, buffer, bufferOffset, count);
            return count;
        }
    }
}
