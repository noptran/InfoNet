using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using LinqKit;

namespace Infonet.Reporting.Filters {
	public class HotlineTwnTshipCountyFilter : ReportFilter {
		public HotlineTwnTshipCountyFilter(string[] towns = null, string[] townships = null, int?[] countyIds = null, string[] zipCodes = null) {
			Label = "Client Location";
			Towns = towns;
			Townships = townships;
			CountyIds = countyIds;
			ZipCodes = zipCodes;
		}

		public string[] Towns { get; set; }

		public string[] Townships { get; set; }

		public int?[] CountyIds { get; set; }

		public string[] ZipCodes { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var result = PredicateBuilder.New<PhoneHotline>(false);
			if (Towns != null)
				result.Or(t => Towns.Contains(t.Town));
			if (Townships != null)
				result.Or(t => Townships.Contains(t.Township));
			if (CountyIds != null)
				result.Or(t => CountyIds.Contains(t.CountyID));
			if (ZipCodes != null)
				result.Or(t => ZipCodes.Contains(t.ZipCode));
			if (result.IsStarted)
				context.PhoneHotline.Predicates.Add(result);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			var criteria = new List<string>();
			if (Towns != null) //KMS DO null is ignored
				criteria.Add($"City or Town is {Towns.ToConjoinedString("or")}");
			if (Townships != null) //KMS DO null is ignored
				criteria.Add($"Township is {Townships.ToConjoinedString("or")}");
			if (CountyIds != null) //KMS DO null is ignored
				criteria.Add($"County is {container.UspsContext.Counties.Where(c => CountyIds.Contains(c.ID)).Select(c => c.CountyName).ToConjoinedString("or")}");
			if (ZipCodes != null) //KMS DO null is ignored  //KMS DO use lookup?
				criteria.Add($"Zip Code is {ZipCodes.ToConjoinedString("or")}");
			if (criteria.Count == 0)
				criteria.Add("<any>");
			w.WriteConjoined(';', "OR", null, criteria);
		}
	}
}