using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class PublicationDetailLocationFilter : LocationFilter {
		public PublicationDetailLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Publication Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PublicationDetail.Predicates.Add(q => LocationIds.Contains(q.CenterID));
		}
	}
}