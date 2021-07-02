using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.OtherStaffActivities {
	public class OSActivityNameReportOrder : ReportOrder<OtherStaffActivity> {

		public override string ReportOrderAsString {
			get {
				return "Activity";
			}
		}

		public override IOrderedQueryable<OtherStaffActivity> ApplyOrder(IQueryable<OtherStaffActivity> query) {
			return query.OrderBy(q => q.TLU_Codes_OtherStaffActivity.Description);
		}

		public override IOrderedQueryable<OtherStaffActivity> ApplyOrder(IOrderedQueryable<OtherStaffActivity> query) {
			return query.ThenBy(q => q.TLU_Codes_OtherStaffActivity.Description);
		}
	}
}
