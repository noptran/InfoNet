using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.OtherStaffActivities {
	public class OSAStaffNameReportOrder : ReportOrder<OtherStaffActivity> {

		public override string ReportOrderAsString {
			get {
				return "Staff Name";
			}
		}

		public override IOrderedQueryable<OtherStaffActivity> ApplyOrder(IQueryable<OtherStaffActivity> query) {
			return query.OrderBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}

		public override IOrderedQueryable<OtherStaffActivity> ApplyOrder(IOrderedQueryable<OtherStaffActivity> query) {
			return query.ThenBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}
	}
}