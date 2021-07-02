using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ClientCenterFilter : LocationFilter {
		public ClientCenterFilter(int?[] locationIds) : base(locationIds) {
			Label = "Client Center";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ClientCase.Predicates.Add(q => LocationIds.Contains(q.Client.CenterId));
		}
	}
}