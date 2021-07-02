using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class EventDetailAddressFilter : ReportFilter {
		public EventDetailAddressFilter(string[] locations) {
			Label = "Event Location";
			Locations = locations;
		}

		public string[] Locations { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.EventDetail.Predicates.Add(q => Locations.Contains(q.Location));
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.WriteConjoined(';', "or", null, Locations);
		}
	}
}