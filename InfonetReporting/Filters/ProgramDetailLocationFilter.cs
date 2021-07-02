using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ProgramDetailLocationFilter : LocationFilter {
		public ProgramDetailLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Program Detail Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ProgramDetail.Predicates.Add(p => LocationIds.Contains(p.CenterID));
		}
	}
}