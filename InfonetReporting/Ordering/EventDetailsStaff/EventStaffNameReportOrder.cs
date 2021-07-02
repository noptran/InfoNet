using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.EventDetailsStaff {
	public class EventStaffNameReportOrder : ReportOrder<EventDetailStaff> {

		public override string ReportOrderAsString {
			get {
				return "Staff Name";
			}
		}

		public override IOrderedQueryable<EventDetailStaff> ApplyOrder(IQueryable<EventDetailStaff> query) {
			return query.OrderBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}

		public override IOrderedQueryable<EventDetailStaff> ApplyOrder(IOrderedQueryable<EventDetailStaff> query) {
			return query.ThenBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}
	}
}