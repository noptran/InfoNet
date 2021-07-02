using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.ProgramDetailsStaff {
	public class ProgramDetailStaffNameReportOrder : ReportOrder<ProgramDetailStaff> {

		public override string ReportOrderAsString {
			get {
				return "Staff Name";
			}
		}

		public override IOrderedQueryable<ProgramDetailStaff> ApplyOrder(IQueryable<ProgramDetailStaff> query) {
			return query.OrderBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}

		public override IOrderedQueryable<ProgramDetailStaff> ApplyOrder(IOrderedQueryable<ProgramDetailStaff> query) {
			return query.ThenBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}
	}
}