using System.Collections.Generic;

namespace Infonet.Reporting.AdHoc {
	public interface IPredicate {
		PredicateOperator Precedence { get; }

		void AddRequiredEntityIdsTo(ISet<string> entityIds);

		void WriteOn(QueryWriter sql);
	}
}