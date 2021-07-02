using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ClientCaseAgeFilter : RangeFilter<int> {
		public ClientCaseAgeFilter(int? from, int? to) : base(from, to) {
			Label = "Age at First Contact";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ClientCase.Predicates.Add(ClientCase.AgeBetween(From, To));
		}
	}
}