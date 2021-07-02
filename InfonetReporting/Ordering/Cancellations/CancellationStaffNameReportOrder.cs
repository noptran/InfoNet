using System.Linq;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.Cancellations {
	public class CancellationStaffNameReportOrder : ReportOrder<Cancellation> {

		public override string ReportOrderAsString {
			get {
				return "Staff Name";
			}
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IQueryable<Cancellation> query) {
			return query.OrderBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}

		public override IOrderedQueryable<Cancellation> ApplyOrder(IOrderedQueryable<Cancellation> query) {
			return query.ThenBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}
	}
}