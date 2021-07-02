using System.Linq;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.Cancellations {
	public class CancellationServiceNameReportOrder : ReportOrder<Cancellation> {

		public override string ReportOrderAsString {
			get {
				return "Service Name";
			}
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IQueryable<Cancellation> query) {
			return query.OrderBy(q => q.TLU_Codes_ProgramsAndServices.Description);
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IOrderedQueryable<Cancellation> query) {
			return query.ThenBy(q => q.TLU_Codes_ProgramsAndServices.Description);
		}
	}
}
