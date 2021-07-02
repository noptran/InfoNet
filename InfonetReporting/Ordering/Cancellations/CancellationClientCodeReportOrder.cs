using System.Linq;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.Cancellations {
	public class CancellationClientCodeReportOrder : ReportOrder<Cancellation> {

		public override string ReportOrderAsString {
			get {
				return "Client ID";
			}
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IQueryable<Cancellation> query) {
			return query.OrderBy(q => q.ClientCase.Client.ClientCode);
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IOrderedQueryable<Cancellation> query) {
			return query.ThenBy(q => q.ClientCase.Client.ClientCode);
		}
	}
}