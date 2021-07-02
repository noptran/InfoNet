using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.PublicationsStaff {
	public class PublicationStaffNameReportOrder : ReportOrder<PublicationDetailStaff> {

		public override string ReportOrderAsString {
			get {
				return "Staff Name";
			}
		}

		public override IOrderedQueryable<PublicationDetailStaff> ApplyOrder(IQueryable<PublicationDetailStaff> query) {
			return query.OrderBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}

		public override IOrderedQueryable<PublicationDetailStaff> ApplyOrder(IOrderedQueryable<PublicationDetailStaff> query) {
			return query.ThenBy(q => q.StaffVolunteer.FirstName + q.StaffVolunteer.LastName);
		}
	}
}