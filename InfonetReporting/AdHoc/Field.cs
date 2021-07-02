using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Infonet.Core.Collections;

namespace Infonet.Reporting.AdHoc {
	[SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
	public class Field : Entity.Child {
		private static readonly Condition[] _IsEmptyConditions = { Condition.IsEmpty, Condition.NotEmpty };
		private static readonly Condition[] _IsAnyOfConditions = { Condition.IsAnyOf, Condition.NotAnyOf };

		private IEnumerable<Condition> _availableConditions = null;

		public Field(Fragment expressionSql, FieldType type, Fragment selectSql = null) : this(null, expressionSql, type, selectSql) { }

		public Field(string localId, Fragment expressionSql, FieldType type, Fragment selectSql = null) : base(localId, expressionSql) {
			if (expressionSql.Parameters.Count > 0)
				throw new ArgumentException($"{nameof(expressionSql)} does not allow {{@}} tags: \"{expressionSql.Source}\"");

			Type = type;
			Comparer = type.Comparer;

			if (selectSql != null) {
				if (selectSql.Ids.Count > 0)
					throw new ArgumentException($"{nameof(selectSql)} does not allow {{#}} tags: \"{selectSql.Source}\"");
				foreach (string each in selectSql.Parameters)
					if (each != "expression")
						throw new ArgumentException($"{nameof(selectSql)} contains unexpected {{@{each}}} tag: \"{selectSql.Source}\"");
				SelectSql = selectSql.Text;
				RequiredEntityIds.AddRange(selectSql.Tags); /* only really required when 'select'ing, but more predicatable to require always */
			}
		}

		public FieldType Type { get; }

		public string SelectSql { get; }

		/*
		 * Originally used to indicate whether a field should have IsEmpty and NotEmpty Conditions
		 * available.  Given that outer joins may make even not-nullable fields null in result sets,
		 * IsEmpty and NotEmpty are now always available.  At this point, this field serves only as
		 * a way of reminding the programmer that Keys must be non-nullable.
		 */
		public bool NotEmpty { get; set; }

		public IComparer Comparer { get; set; }

		public IEnumerable<Condition> AvailableConditions {
			get {
				if (_availableConditions != null)
					return _availableConditions;

				var result = Type.Conditions.Concat(_IsEmptyConditions);
				if (Options != null || OptionsSql != null)
					result = _IsAnyOfConditions.Concat(result);
				return result;
			}
			set { _availableConditions = value; }
		}

		public IEnumerable<Option> Options { get; set; }

		public string OptionsSql { get; set; }

		public Func<object, string> Formatter { get; set; }

		public string Format(object value) {
			return Formatter != null ? Formatter(value) : Type.Format(value);
		}

		public virtual void WriteToSelect(QueryWriter sql) {
			if (SelectSql == null)
				sql.Write(ExpressionSql);
			else
				sql.Write(SelectSql, ExpressionSql);
			if (Id != null) {
				sql.Write(" AS ");
				sql.Write(Id);
			}
		}

		public virtual void WriteToPredicate(QueryWriter sql) {
			sql.Write(ExpressionSql);
		}

		public virtual void WriteToOrderBy(QueryWriter sql, bool descending) {
			sql.Write(ExpressionSql);
			if (descending)
				sql.Write(" DESC");
		}

		public virtual IFieldReader CreateReader() {
			return new Reader(this);
		}

		#region inner
		internal class Reader : IFieldReader {
			private int _ordinal = -1;
			private readonly WeakReference<SqlDataReader> _lastReader = new WeakReference<SqlDataReader>(null);

			internal Reader(Field field) {
				Field = field;
			}

			public Field Field { get; }

			public virtual object Read(SqlDataReader reader) {
				SqlDataReader last;
				if (_ordinal < 0 || !_lastReader.TryGetTarget(out last) || last != reader) {
					_ordinal = reader.GetOrdinal(Field.Id);
					_lastReader.SetTarget(reader);
				}
				var result = reader.GetValue(_ordinal);
				return result == DBNull.Value ? null : result;
			}
		}
		#endregion
	}
}