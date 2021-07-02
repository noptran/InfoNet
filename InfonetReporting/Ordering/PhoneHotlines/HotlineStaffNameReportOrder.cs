using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.PhoneHotlines {
	public class HotlineStaffNameReportOrder : ReportOrder<PhoneHotline> {

		public override string ReportOrderAsString {
			get {
				return "Staff Name";
			}
		}

		public override IOrderedQueryable<PhoneHotline> ApplyOrder(IQueryable<PhoneHotline> query) {
			return query.OrderBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}

		public override IOrderedQueryable<PhoneHotline> ApplyOrder(IOrderedQueryable<PhoneHotline> query) {
			return query.ThenBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}
	}
}