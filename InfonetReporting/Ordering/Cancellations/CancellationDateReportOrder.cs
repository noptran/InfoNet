using System.Linq;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.Cancellations {
	public class CancellationDateReportOrder : ReportOrder<Cancellation> {

		public override string ReportOrderAsString {
			get {
				return "Date";
			}
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IQueryable<Cancellation> query) {
			return query.OrderBy(q => q.Date);
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IOrderedQueryable<Cancellation> query) {
			return query.ThenBy(q => q.Date);
		}
	}
}