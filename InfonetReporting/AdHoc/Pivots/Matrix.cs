using System;
using System.IO;

namespace Infonet.Reporting.AdHoc.Pivots {
	public class Matrix<TEntry> {
		public const int DEFAULT_ROW_CAPACITY = 4;
		public const int DEFAULT_COLUMN_CAPACITY = 4;

		private TEntry[,] _entries;
		private int _rowCount = 0;
		private int _columnCount = 0;

		public Matrix(int initialRowCapacity = DEFAULT_ROW_CAPACITY, int initialColumnCapacity = DEFAULT_COLUMN_CAPACITY) {
			EnsureCapacity(initialRowCapacity, initialColumnCapacity);
		}

		public int RowCapacity {
			get { return _entries?.GetLength(0) ?? 0; }
		}

		public int ColumnCapacity {
			get { return _entries?.GetLength(1) ?? 0; }
		}

		public int RowCount {
			get { return _rowCount; }
		}

		public int ColumnCount {
			get { return _columnCount; }
		}

		public TEntry this[int row, int column] {
			get {
				if (row < 0 || row >= _rowCount)
					throw new IndexOutOfRangeException($"Argument {nameof(row)}(={row}) is out of range");
				if (column < 0 || column >= _columnCount)
					throw new IndexOutOfRangeException($"Argument {nameof(column)}(={column}) is out of range");
				return _entries[row, column];
			}
			set {
				if (row < 0 || row >= _rowCount)
					throw new IndexOutOfRangeException($"Argument {nameof(row)}(={row}) is out of range");
				if (column < 0 || column >= _columnCount)
					throw new IndexOutOfRangeException($"Argument {nameof(column)}(={column}) is out of range");
				_entries[row, column] = value;
			}
		}

		public void GrowBy(int rows, int columns) {
			if (rows == 0 && columns == 0)
				return;

			int newRows = _rowCount + rows;
			int newColumns = _columnCount + columns;
			EnsureCapacity(newRows, newColumns);
			_rowCount = newRows;
			_columnCount = newColumns;
		}

		public override string ToString() {
			using (var w = new StringWriter()) {
				w.Write("{");
				for (int row = 0; row < RowCount; row++) {
					if (RowCount > 1)
						w.WriteLine();
					w.Write("  {");
					for (int column = 0; column < ColumnCount; column++) {
						object each = _entries[row, column];
						if (column != 0)
							w.Write(",");
						w.Write(" ");
						if (each == null)
							w.Write("--");
						else
							w.Write(_entries[row, column]);
					}
					w.Write(" }");
				}
				if (RowCount > 1)
					w.WriteLine();
				else
					w.Write("  ");
				w.Write("}");
				return w.ToString();
			}
		}

		//KMS DO remove unused?
		//public Matrix<TResult> Transform<TResult>(Func<TEntry, TResult> transform) {
		//	var result = new Matrix<TResult>(RowCapacity, ColumnCapacity);
		//	for (int row = 0; row < RowCount; row++)
		//		for (int column = 0; column < ColumnCount; column++)
		//			result[row, column] = transform(this[row, column]);
		//	return result;
		//}

		#region private
		private void EnsureCapacity(int rows, int columns) {
			int newRowCount = rows <= RowCapacity ? RowCapacity : Math.Max(rows, RowCapacity * 2);
			int newColumnCount = columns <= ColumnCapacity ? ColumnCapacity : Math.Max(columns, ColumnCapacity * 2);
			if (newRowCount == RowCapacity && newColumnCount == ColumnCapacity)
				return;

			var newEntries = new TEntry[newRowCount, newColumnCount];
			if (_entries != null)
				for (int row = 0; row < RowCount; row++)
					for (int column = 0; column < ColumnCount; column++)
						newEntries[row, column] = _entries[row, column];
			_entries = newEntries;
		}
		#endregion
	}
}