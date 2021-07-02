using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

//KMS DO rename namspace to Pivoting?
namespace Infonet.Reporting.AdHoc.Pivots {
	public class QueryPivot {
		private readonly PivotTable<SqlDataReader, object> _table;
		private readonly IFieldReader[] _rowDimensions;
		private readonly IFieldReader[] _columnDimensions;
		private readonly object[] _rowBuffer;
		private readonly object[] _columnBuffer;

		public QueryPivot(IEnumerable<Field> rowDimensions = null, IEnumerable<Field> columnDimensions = null, Func<IAggregate<SqlDataReader, object>> aggregateTemplate = null) {
			_rowDimensions = (rowDimensions ?? Array.Empty<Field>()).Select(f => f.CreateReader()).ToArray();
			_columnDimensions = (columnDimensions ?? Array.Empty<Field>()).Select(f => f.CreateReader()).ToArray();
			_table = new PivotTable<SqlDataReader, object>(_rowDimensions.Length, _columnDimensions.Length, aggregateTemplate ?? (() => new Count()));
			foreach (var each in _table.RowDimensions) {
				var eachField = GetRowDimension(each.CoordinateOrdinal);
				each.Label = eachField.Label;
				each.Comparer = eachField.Comparer;
			}
			foreach (var each in _table.ColumnDimensions) {
				var eachField = GetColumnDimension(each.CoordinateOrdinal);
				each.Label = eachField.Label;
				each.Comparer = eachField.Comparer;
			}
			_rowBuffer = new object[_rowDimensions.Length];
			_columnBuffer = new object[_columnDimensions.Length];
		}

		public string Caption { get; set; }

		public Field GetRowDimension(int ordinal) {
			return _rowDimensions[ordinal].Field;
		}

		public Field GetColumnDimension(int ordinal) {
			return _columnDimensions[ordinal].Field;
		}

		public void Ingest(SqlDataReader reader) {
			for (int i = 0; i < _rowBuffer.Length; i++)
				_rowBuffer[i] = _rowDimensions[i].Read(reader);
			for (int i = 0; i < _columnBuffer.Length; i++)
				_columnBuffer[i] = _columnDimensions[i].Read(reader);

			var row = new Coordinate(_rowBuffer);
			var column = new Coordinate(_columnBuffer);
			_table.Ingest(row, column, reader);
		}

		//KMS DO name? still needed here?
		public PivotTable<SqlDataReader, object> ToTable() {
			_table.SortRowsByCoordinates();
			_table.SortColumnsByCoordinates();
			_table.Caption = Caption;
			return _table;
		}
	}
}