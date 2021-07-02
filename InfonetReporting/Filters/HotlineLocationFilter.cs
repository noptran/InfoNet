using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class HotlineLocationFilter : LocationFilter {
		public HotlineLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Hotline Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PhoneHotline.Predicates.Add(ph => LocationIds.Contains(ph.CenterID));
		}
	}
}