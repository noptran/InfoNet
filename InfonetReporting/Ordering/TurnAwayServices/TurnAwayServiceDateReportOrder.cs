using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.TurnAwayServices {
	public class TurnAwayServiceDateReportOrder : ReportOrder<TurnAwayService> {
		public override string ReportOrderAsString {
			get {
				return "Turn Away Date";
			}
		}

		public override IOrderedQueryable<TurnAwayService> ApplyOrder(IQueryable<TurnAwayService> query) {
			return query.OrderBy(q => q.TurnAwayDate);
		}

		public override IOrderedQueryable<TurnAwayService> ApplyOrder(IOrderedQueryable<TurnAwayService> query) {
			return query.ThenBy(q => q.TurnAwayDate);
		}
	}
}