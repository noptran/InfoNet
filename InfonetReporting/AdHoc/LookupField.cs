using System.Data.SqlClient;
using System.Linq;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Data.Looking;

namespace Infonet.Reporting.AdHoc {
	public class LookupField : Field {
		public LookupField(Fragment expressionSql, LookupGroup lookup) : this(null, expressionSql, lookup) { }

		public LookupField(string localId, Fragment expressionSql, LookupGroup lookup) : base(localId, expressionSql, FieldType.Lookup) {
			Lookup = lookup;
			Comparer = lookup.Comparer;
			Options = lookup.Select(lc => new Option { Value = lc.CodeId, Label = lc.Description });
		}

		private LookupGroup Lookup { get; }

		public override void WriteToOrderBy(QueryWriter sql, bool descending) {
			sql.Write("(SELECT displayOrder FROM ");
			sql.Write(Lookup.Source.TableName);
			sql.Write(" WHERE tableId = ");
			sql.Write(Lookup.Source.TableId.ToString());
			sql.Write(" AND providerId = ");
			sql.Write(Lookup.Provider.ToInt32().ToString());
			sql.Write(" AND codeId = ");
			sql.Write(ExpressionSql);
			sql.Write(")");
			if (descending)
				sql.Write(" DESC");
			sql.Write("(SELECT description FROM ");
			sql.Write(Lookup.Source.TableName);
			sql.Write(" WHERE codeId = ");
			sql.Write(ExpressionSql);
			sql.Write(")");
			if (descending)
				sql.Write(" DESC");
		}

		public override IFieldReader CreateReader() {
			return new LookupReader(this, Lookup.Source);
		}

		#region inner
		private class LookupReader : Reader {
			private readonly Lookup _lookup;

			internal LookupReader(Field field, Lookup lookup) : base(field) {
				_lookup = lookup;
			}

			public override object Read(SqlDataReader reader) {
				return _lookup[ConvertNull.ToInt32(base.Read(reader))];
			}
		}
		#endregion
	}
}