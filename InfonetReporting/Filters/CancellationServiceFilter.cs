using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class CancellationServiceFilter : LookupFilter {
		public CancellationServiceFilter(int?[] codeIds = null) : base(Lookups.ProgramsAndServices, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.Cancellation.Predicates.Add(q => CodeIds.Contains(q.ServiceID));
		}
	}
}