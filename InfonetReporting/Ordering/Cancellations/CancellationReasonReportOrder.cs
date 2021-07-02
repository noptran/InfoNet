using System.Linq;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.Cancellations {
	public class CancellationReasonReportOrder : ReportOrder<Cancellation> {

		public override string ReportOrderAsString {
			get {
				return "Reason";
			}
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IQueryable<Cancellation> query) {
			return query.OrderBy(q => q.ReasonID);
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IOrderedQueryable<Cancellation> query) {
			return query.ThenBy(q => q.ReasonID);
		}
	}
}
