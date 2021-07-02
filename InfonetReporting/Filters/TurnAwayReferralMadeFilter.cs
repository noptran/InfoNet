using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class TurnAwayReferralMadeFilter : LookupFilter {
		public TurnAwayReferralMadeFilter(int?[] codeIds = null) : base(Lookups.YesNo2, codeIds) {
			Label = "Referral Made";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.TurnAwayService.Predicates.Add(q => CodeIds.Contains(q.ReferralMadeId));
		}
	}
}