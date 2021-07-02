using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ServiceDetailServiceFilter : LookupFilter {
		public ServiceDetailServiceFilter(int?[] codeIds = null) : base(Lookups.ProgramsAndServices, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ServiceDetailOfClient.Predicates.Add(t => CodeIds.Contains(t.ServiceID));
		}
	}
}