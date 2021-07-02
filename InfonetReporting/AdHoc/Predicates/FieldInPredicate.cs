using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;

namespace Infonet.Reporting.AdHoc.Predicates {
	public class FieldInPredicate : FieldPredicate {
		private IEnumerable<object> _in = new object[0];

		public FieldInPredicate(Field field) : base(field) { }

		public IEnumerable<object> In {
			get { return _in; }
			set { _in = value.Select(o => Field.Type.Convert(o)); }
		}

		public override void WriteOn(QueryWriter sql) {
			Field.WriteToPredicate(sql);
			if (Not)
				sql.Write(" NOT");
			sql.Write(" IN (");
			for (var en = Lookahead.New(In); en.MoveNext();) {
				if (!en.IsFirst)
					sql.Write(", ");
				sql.WriteParameter(en.Current, Field, "in");
			}
			sql.Write(")");
		}
	}
}