using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.EventDetailsStaff {
	public class EventDateReportOrder : ReportOrder<EventDetailStaff> {

		public override string ReportOrderAsString {
			get {
				return "Date";
			}
		}

		public override IOrderedQueryable<EventDetailStaff> ApplyOrder(IQueryable<EventDetailStaff> query) {
			return query.OrderBy(q => q.EventDetail.EventDate);
		}

		public override IOrderedQueryable<EventDetailStaff> ApplyOrder(IOrderedQueryable<EventDetailStaff> query) {
			return query.ThenBy(q => q.EventDetail.EventDate);
		}
	}
}