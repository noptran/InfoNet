using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class CancellationReasonFilter : LookupFilter {
		public CancellationReasonFilter(int?[] codeIds = null) : base(Lookups.CancellationReason, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.Cancellation.Predicates.Add(q => CodeIds.Contains(q.ReasonID));
		}
	}
}