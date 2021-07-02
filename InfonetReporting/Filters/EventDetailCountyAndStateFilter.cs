using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using LinqKit;

namespace Infonet.Reporting.Filters {
	public class EventDetailCountyAndStateFilter : ReportFilter {
		public EventDetailCountyAndStateFilter(int?[] countyIds = null, int?[] stateIds = null) {
			Label = "Event Location";
			CountyIds = countyIds;
			StateIds = stateIds;
		}

		public int?[] CountyIds { get; set; }

		public int?[] StateIds { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var result = PredicateBuilder.New<EventDetail>(false);
			if (CountyIds != null)
				result.Or(t => CountyIds.Contains(t.CountyID));
			if (StateIds != null)
				result.Or(t => StateIds.Contains(t.StateID));
			if (result.IsStarted)
				context.EventDetail.Predicates.Add(result);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			var criteria = new List<string>();
			if (CountyIds != null) //KMS DO null is ignored
				criteria.Add($"County is {container.UspsContext.Counties.Where(c => CountyIds.Contains(c.ID)).Select(c => c.CountyName).ToConjoinedString("or")}");
			if (StateIds != null) //KMS DO null is ignored  //KMS DO use lookup?
				criteria.Add($"State is {container.UspsContext.States.Where(s => StateIds.Contains(s.ID)).Select(s => s.StateName).ToConjoinedString("or")}");
			if (criteria.Count == 0)
				criteria.Add("<any>");
			w.WriteConjoined(';', "OR", null, criteria);
		}
	}
}