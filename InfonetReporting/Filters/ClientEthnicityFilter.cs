using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ClientEthnicityFilter : LookupFilter {
		public ClientEthnicityFilter(int?[] codeIds = null) : base(Lookups.Ethnicity, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.Client.Predicates.Add(t => CodeIds.Contains(t.EthnicityId));
		}
	}
}