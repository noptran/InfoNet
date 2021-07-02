using System.Collections.Generic;

namespace Infonet.Reporting.AdHoc.Predicates {
	public abstract class FieldPredicate : IPredicate {
		protected FieldPredicate(Field field) {
			Field = field;
		}

		public Field Field { get; }

		public bool Not { get; set; }

		public virtual PredicateOperator Precedence {
			get { return PredicateOperator.Comparison; }
		}

		public void AddRequiredEntityIdsTo(ISet<string> entityIds) {
			Field.AddRequiredEntityIdsTo(entityIds);
		}

		public abstract void WriteOn(QueryWriter sql);
	}
}