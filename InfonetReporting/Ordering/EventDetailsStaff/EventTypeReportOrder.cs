using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.EventDetailsStaff {
	public class EventTypeReportOrder : ReportOrder<EventDetailStaff> {

		public override string ReportOrderAsString {
			get {
				return "Event Type";
			}
		}

		public override IOrderedQueryable<EventDetailStaff> ApplyOrder(IQueryable<EventDetailStaff> query) {
			return query.OrderBy(q => q.EventDetail.TLU_Codes_ProgramsAndServices.Description);
		}

		public override IOrderedQueryable<EventDetailStaff> ApplyOrder(IOrderedQueryable<EventDetailStaff> query) {
			return query.ThenBy(q => q.EventDetail.TLU_Codes_ProgramsAndServices.Description);
		}
	}
}
