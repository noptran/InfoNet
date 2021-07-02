namespace Infonet.Reporting.AdHoc.Predicates {
	public class FieldBetweenPredicate : FieldPredicate {
		private object _min = null;
		private object _max = null;

		public FieldBetweenPredicate(Field field) : base(field) { }

		public override PredicateOperator Precedence {
			get {
				if (Min == null || Max == null || !MinExcluded && !MaxExcluded)
					return PredicateOperator.Comparison;
				return Not ? PredicateOperator.Or : PredicateOperator.And;
			}
		}

		public bool MinExcluded { get; set; }

		public bool MaxExcluded { get; set; }

		public object Min {
			get { return _min; }
			set { _min = Field.Type.Convert(value); }
		}

		public object Max {
			get { return _max; }
			set { _max = Field.Type.Convert(value); }
		}

		public override void WriteOn(QueryWriter sql) {
			if (Min == null && Max == null) {
				sql.Write("1 = 1");
				return;
			}
			if (Min != null && Max != null && !MinExcluded && !MaxExcluded) {
				Field.WriteToPredicate(sql);
				if (Not)
					sql.Write(" NOT");
				sql.Write(" BETWEEN ");
				sql.WriteParameter(Min, Field, "min");
				sql.Write(" AND ");
				sql.WriteParameter(Max, Field, "max");
				return;
			}
			Field.WriteToPredicate(sql);
			if (Min != null) {
				if (!MinExcluded)
					sql.Write(Not ? " < " : " >= ");
				else
					sql.Write(Not ? " <= " : " > ");
				sql.WriteParameter(Min, Field, "min");
			}
			if (Max != null) {
				if (Min != null)
					sql.Write(Not ? " OR " : " AND ");
				if (!MaxExcluded)
					sql.Write(Not ? " > " : " <= ");
				else
					sql.Write(Not ? " >= " : " < ");
				sql.WriteParameter(Max, Field, "max");
			}
		}
	}
}