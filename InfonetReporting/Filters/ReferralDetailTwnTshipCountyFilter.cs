using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ReferralDetailTwnTshipCountyFilter : TwnTshipCountyFilter {
		public ReferralDetailTwnTshipCountyFilter(string[] cityOrTowns = null, string[] townships = null, int?[] countyIds = null, int?[] stateIds = null, string[] zipCodes = null) : base(cityOrTowns, townships, countyIds, stateIds, zipCodes) { }

		protected override Vertex<TwnTshipCounty> SelectVertex(FilterContext context) {
			return context.ClientReferralDetail.TwnTshipCounty;
		}
	}
}