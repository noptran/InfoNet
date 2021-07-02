using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.OtherStaffActivities {
	public class OSADateReportOrder : ReportOrder<OtherStaffActivity> {

		public override string ReportOrderAsString {
			get {
				return "Date";
			}
		}

		public override IOrderedQueryable<OtherStaffActivity> ApplyOrder(IQueryable<OtherStaffActivity> query) {
			return query.OrderBy(q => q.OsaDate);
		}

		public override IOrderedQueryable<OtherStaffActivity> ApplyOrder(IOrderedQueryable<OtherStaffActivity> query) {
			return query.ThenBy(q => q.OsaDate);
		}
	}
}