using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class EventDetailProgramFilter : LookupFilter {
		public EventDetailProgramFilter(int?[] codeIds = null) : base(Lookups.ProgramsAndServices, codeIds) {
			Label = "Event Type";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.EventDetail.Predicates.Add(q => CodeIds.Contains(q.ProgramID));
		}
	}
}