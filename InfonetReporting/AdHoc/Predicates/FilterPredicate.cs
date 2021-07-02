using System.Collections.Generic;

namespace Infonet.Reporting.AdHoc.Predicates {
	public class FilterPredicate : IPredicate {
		public FilterPredicate(Filter filter) {
			Filter = filter;
		}

		public Filter Filter { get; }

		public IDictionary<string, object> Criteria { get; set; }

		public PredicateOperator Precedence {
			get { return Filter.Precedence; }
		}

		public void AddRequiredEntityIdsTo(ISet<string> entityIds) {
			Filter.AddRequiredEntityIdsTo(entityIds);
		}

		public void WriteOn(QueryWriter sql) {
			Filter.WriteOn(sql, Criteria);
		}
	}
}