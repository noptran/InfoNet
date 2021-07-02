using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ServiceDetailTwnTshipCountyFilter : TwnTshipCountyFilter {
		public ServiceDetailTwnTshipCountyFilter(string[] cityOrTowns = null, string[] townships = null, int?[] countyIds = null, int?[] stateIds = null, string[] zipCodes = null) : base(cityOrTowns, townships, countyIds, stateIds, zipCodes) { }

		protected override Vertex<TwnTshipCounty> SelectVertex(FilterContext context) {
			return context.ServiceDetailOfClient.TwnTshipCounty;
		}
	}
}