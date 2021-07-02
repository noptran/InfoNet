using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using LinqKit;

namespace Infonet.Reporting.Filters {
	public abstract class TwnTshipCountyFilter : ReportFilter {
		protected TwnTshipCountyFilter(string[] cityOrTowns = null, string[] townships = null, int?[] countyIds = null, int?[] stateIds = null, string[] zipCodes = null) {
			Label = "Client Location";
			CityOrTowns = cityOrTowns;
			Townships = townships;
			CountyIds = countyIds;
			StateIds = stateIds;
			ZipCodes = zipCodes;
		}

		public string[] CityOrTowns { get; set; }

		public string[] Townships { get; set; }

		public int?[] CountyIds { get; set; }

		public int?[] StateIds { get; set; }

		public string[] ZipCodes { get; set; }

		protected abstract Vertex<TwnTshipCounty> SelectVertex(FilterContext context);

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var result = PredicateBuilder.New<TwnTshipCounty>(false);
			if (CityOrTowns != null)
				result.Or(t => CityOrTowns.Contains(t.CityOrTown));
			if (Townships != null)
				result.Or(t => Townships.Contains(t.Township));
			if (CountyIds != null)
				result.Or(t => CountyIds.Contains(t.CountyID));
			if (StateIds != null)
				result.Or(t => StateIds.Contains(t.StateID));
			if (ZipCodes != null)
				result.Or(t => ZipCodes.Contains(t.Zipcode));
			if (result.IsStarted)
				SelectVertex(context).Predicates.Add(result);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			var criteria = new List<string>();
			if (CityOrTowns != null) //KMS DO null is ignored
				criteria.Add($"City or Town is {CityOrTowns.ToConjoinedString("or")}");
			if (Townships != null) //KMS DO null is ignored
				criteria.Add($"Township is {Townships.ToConjoinedString("or")}");
			if (CountyIds != null) //KMS DO null is ignored
				criteria.Add($"County is {container.UspsContext.Counties.Where(c => CountyIds.Contains(c.ID)).Select(c => c.CountyName).ToConjoinedString("or")}");
			if (StateIds != null) //KMS DO null is ignored  //KMS DO use lookup?
				criteria.Add($"State is {container.UspsContext.States.Where(s => StateIds.Contains(s.ID)).Select(s => s.StateName).ToConjoinedString("or")}");
			if (ZipCodes != null) //KMS DO null is ignored  //KMS DO use lookup?
				criteria.Add($"Zip Code is {ZipCodes.ToConjoinedString("or")}");
			if (criteria.Count == 0)
				criteria.Add("<any>");
			w.WriteConjoined(';', "OR", null, criteria);
		}
	}
}