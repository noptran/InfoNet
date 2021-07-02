using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Infonet.Core.Collections;

namespace Infonet.Reporting.AdHoc.Pivots {
	public class PivotTable<TInput, TCell> {
		private readonly Func<IAggregate<TInput, TCell>> _aggregateTemplate;
		private readonly Infonet.Core.Collections.KeyedCollection<Coordinate, Row> _rows = new Infonet.Core.Collections.KeyedCollection<Coordinate, Row>(v => v.Coordinate);
		private readonly Infonet.Core.Collections.KeyedCollection<Coordinate, Column> _columns = new Infonet.Core.Collections.KeyedCollection<Coordinate, Column>(v => v.Coordinate);
		private readonly Matrix<IAggregate<TInput, TCell>> _cells;
		private readonly IAggregate<TInput, TCell> _aggregate;
		private readonly IAggregate<TInput, TCell> _empty;
		private ReadOnlyCollection<Row> _rowsReadOnly;
		private ReadOnlyCollection<Column> _columnsReadOnly;

		public PivotTable(int rowDimensions, int columnDimensions, Func<IAggregate<TInput, TCell>> aggregateTemplate, int initialRowCapacity = Matrix<TCell>.DEFAULT_ROW_CAPACITY, int initialColumnCapacity = Matrix<TCell>.DEFAULT_COLUMN_CAPACITY) {
			RowDimensions = Array.AsReadOnly(Enumerable.Range(0, rowDimensions).Select(i => new Dimension(i)).ToArray());
			ColumnDimensions = Array.AsReadOnly(Enumerable.Range(0, columnDimensions).Select(i => new Dimension(i)).ToArray());
			_aggregateTemplate = aggregateTemplate;
			_cells = new Matrix<IAggregate<TInput, TCell>>(initialRowCapacity, initialColumnCapacity);
			_aggregate = _aggregateTemplate();
			_empty = _aggregateTemplate();
		}

		public ReadOnlyCollection<Dimension> RowDimensions { get; }

		public ReadOnlyCollection<Dimension> ColumnDimensions { get; }

		public ReadOnlyCollection<Row> Rows {
			get { return _rowsReadOnly ?? (_rowsReadOnly = new ReadOnlyCollection<Row>(_rows)); }
		}

		public ReadOnlyCollection<Column> Columns {
			get { return _columnsReadOnly ?? (_columnsReadOnly = new ReadOnlyCollection<Column>(_columns)); }
		}

		public TCell this[Coordinate row, Coordinate column] {
			get {
				if (row == null)
					throw new ArgumentNullException(nameof(row));
				if (column == null)
					throw new ArgumentNullException(nameof(column));

				return this[_rows[row].MatrixOrdinal, _columns[column].MatrixOrdinal];
			}
		}

		internal TCell this[int row, int column] {
			get {
				var aggregate = _cells[row, column];
				return aggregate == null ? _empty.Result : aggregate.Result;
			}
		}

		public TCell Total {
			get { return _aggregate.Result; }
		}

		/* Optionally specify the table's caption for external display purposes (not used internally) */
		public string Caption { get; set; }

		public override string ToString() {
			using (var w = new StringWriter()) {
				w.Write(GetType().Name);
				w.WriteLine("[");
				//KMS DO Dimensions?
				w.Write("  rowHeaders: ");
				w.WriteLine(string.Join(", ", _rows.Select(v => v.Coordinate)));
				w.Write("  rowTotals: ");
				w.WriteLine(string.Join(", ", _rows.Select(v => v.Aggregate)));
				w.Write("  columnHeaders: ");
				w.WriteLine(string.Join(", ", _columns.Select(v => v.Coordinate)));
				w.Write("  columnTotals: ");
				w.WriteLine(string.Join(", ", _columns.Select(v => v.Aggregate)));
				w.Write("  cells: ");
				w.Write(_cells);
				w.Write("]");
				return w.ToString();
			}
		}

		public void Ingest(Coordinate row, Coordinate column, TInput input) {
			if (row == null)
				throw new ArgumentNullException(nameof(row));
			if (column == null)
				throw new ArgumentNullException(nameof(column));
			if (row.Rank != RowDimensions.Count || column.Rank != ColumnDimensions.Count)
				throw new ArgumentException($"{nameof(Coordinate)}.{nameof(Coordinate.Rank)} must equal Row/{nameof(ColumnDimensions)}.{nameof(ReadOnlyCollection<Dimension>.Count)}");

			Row rv;
			Column cv;
			GetOrCreateVectors(row, column, out rv, out cv);
			_aggregate.Ingest(input);
			rv.Aggregate.Ingest(input);
			cv.Aggregate.Ingest(input);
			var aggregate = _cells[rv.MatrixOrdinal, cv.MatrixOrdinal] ?? (_cells[rv.MatrixOrdinal, cv.MatrixOrdinal] = _aggregateTemplate());
			aggregate.Ingest(input);
		}

		public void SortRowsByCoordinates() {
			var comparer = new CoordinateComparer(RowDimensions.Select(d => d.Comparer));
			var sortedRows = _rows.OrderBy(r => r.Coordinate, comparer).ToArray();
			_rows.Clear();
			foreach (var each in sortedRows)
				_rows.Add(each);
		}

		public void SortColumnsByCoordinates() {
			var comparer = new CoordinateComparer(ColumnDimensions.Select(d => d.Comparer));
			var sortedColumns = _columns.OrderBy(r => r.Coordinate, comparer).ToArray();
			_columns.Clear();
			foreach (var each in sortedColumns)
				_columns.Add(each);
		}

		private void GetOrCreateVectors(Coordinate row, Coordinate column, out Row rv, out Column cv) {
			_rows.TryGetValue(row, out rv);
			_columns.TryGetValue(column, out cv);
			_cells.GrowBy(rv == null ? 1 : 0, cv == null ? 1 : 0);
			if (rv == null)
				_rows.Add(rv = new Row(this, row, _cells.RowCount - 1, _aggregateTemplate()));
			if (cv == null)
				_columns.Add(cv = new Column(this, column, _cells.ColumnCount - 1, _aggregateTemplate()));
		}

		#region inner
		public class Dimension {
			internal Dimension(int coordinateOrdinal) {
				CoordinateOrdinal = coordinateOrdinal;
			}

			public int CoordinateOrdinal { get; }

			public string Label { get; set; }

			public IComparer Comparer { get; set; }

			//KMS DO if included orientation, could reference _rows/_columns and enumerate from here...but i'm not sure what that means yet
		}

		public abstract class Vector<TVector, TCrossVector> : IEnumerable<KeyValuePair<TCrossVector, TCell>> where TVector : Vector<TVector, TCrossVector> where TCrossVector : Vector<TCrossVector, TVector> {
			protected Vector(PivotTable<TInput, TCell> owner, Coordinate coordinate, int matrixOrdinal, IAggregate<TInput, TCell> aggregate) {
				Owner = owner;
				Coordinate = coordinate;
				MatrixOrdinal = matrixOrdinal;
				Aggregate = aggregate;
			}

			protected PivotTable<TInput, TCell> Owner { get; }

			public Coordinate Coordinate { get; }

			internal int MatrixOrdinal { get; }

			internal IAggregate<TInput, TCell> Aggregate { get; }

			public TCell Total {
				get { return Aggregate.Result; }
			}

			protected abstract IEnumerable<TCrossVector> CrossVectors { get; }

			protected abstract TCell this[int ordinal] { get; }

			public IEnumerator<KeyValuePair<TCrossVector, TCell>> GetEnumerator() {
				return new Enumerator(this);
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			private class Enumerator : EnumeratorDecorator<TCrossVector, KeyValuePair<TCrossVector, TCell>> {
				private readonly Vector<TVector, TCrossVector> _owner;

				internal Enumerator(Vector<TVector, TCrossVector> owner) {
					_owner = owner;
					Inner = owner.CrossVectors.GetEnumerator();
				}

				protected override IEnumerator<TCrossVector> Inner { get; }

				public override KeyValuePair<TCrossVector, TCell> Current {
					get {
						var current = Inner.Current;
						return new KeyValuePair<TCrossVector, TCell>(current, _owner[current.MatrixOrdinal]);
					}
				}
			}
		}

		public class Row : Vector<Row, Column> {
			internal Row(PivotTable<TInput, TCell> owner, Coordinate coordinate, int matrixOrdinal, IAggregate<TInput, TCell> aggregate) : base(owner, coordinate, matrixOrdinal, aggregate) { }

			protected override IEnumerable<Column> CrossVectors {
				get { return Owner.Columns; }
			}

			protected override TCell this[int columnOrdinal] {
				get { return Owner[MatrixOrdinal, columnOrdinal]; }
			}
		}

		public class Column : Vector<Column, Row> {
			internal Column(PivotTable<TInput, TCell> owner, Coordinate coordinate, int matrixOrdinal, IAggregate<TInput, TCell> aggregate) : base(owner, coordinate, matrixOrdinal, aggregate) { }

			protected override IEnumerable<Row> CrossVectors {
				get { return Owner.Rows; }
			}

			protected override TCell this[int rowOrdinal] {
				get { return Owner[rowOrdinal, MatrixOrdinal]; }
			}
		}
		#endregion
	}
}