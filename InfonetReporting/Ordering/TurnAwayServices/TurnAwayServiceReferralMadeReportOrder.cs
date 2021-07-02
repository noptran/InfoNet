using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.TurnAwayServices {
	public class TurnAwayServiceReferralMadeReportOrder : ReportOrder<TurnAwayService> {
		public override string ReportOrderAsString {
			get {
				return "Referral Made";
			}
		}

		public override IOrderedQueryable<TurnAwayService> ApplyOrder(IQueryable<TurnAwayService> query) {
			return query.OrderBy(q => q.ReferralMadeId);
		}

		public override IOrderedQueryable<TurnAwayService> ApplyOrder(IOrderedQueryable<TurnAwayService> query) {
			return query.ThenBy(q => q.ReferralMadeId);
		}
	}
}