using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ClientGenderIdentityFilter : LookupFilter {
		public ClientGenderIdentityFilter(int?[] codeIds = null) : base(Lookups.GenderIdentity, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.Client.Predicates.Add(t => CodeIds.Contains(t.GenderIdentityId));
		}
	}
}