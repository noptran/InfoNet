using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Reporting.AdHoc.Predicates;

namespace Infonet.Reporting.AdHoc {
	public class Filter : Entity.Child {
		public Filter(string parentId, string localId, Fragment expressionSql, string label, bool skipParentTagCheck = false) : base(localId, expressionSql) {
			Precedence = PredicateOperator.Comparison;
			ParameterNames = expressionSql.Parameters;
			SetParentId(parentId, skipParentTagCheck);
			Label = label;
		}

		public IReadOnlyCollection<string> ParameterNames { get; } //KMS DO requires ReadOnly?  maybe use KeyedCollection here...

		public PredicateOperator Precedence { get; set; }

		public FilterPredicate ToPredicate(IDictionary<string, object> criteria = null) {
			return new FilterPredicate(this) { Criteria = criteria };
		}

		public void WriteOn(QueryWriter w, IDictionary<string, object> criteria) {
			if (criteria == null && ParameterNames.Count > 0)
				throw new ArgumentNullException(nameof(criteria));
			//KMS DO error if criteria contains extras?

			string prefix = Id + Model.ID_SEPARATOR;
			w.Write(ExpressionSql, ParameterNames.Select(n => (object)w.AddParameter(criteria[n], null, prefix + n)).ToArray());

			//KMS DO don't forget list handling...if object is IEnumerable...
		}
	}
}