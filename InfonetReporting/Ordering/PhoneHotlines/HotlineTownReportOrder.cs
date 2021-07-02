using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.PhoneHotlines {
	public class HotlineTownReportOrder : ReportOrder<PhoneHotline> {

		public override string ReportOrderAsString {
			get {
				return "Town";
			}
		}

		public override IOrderedQueryable<PhoneHotline> ApplyOrder(IQueryable<PhoneHotline> query) {
			return query.OrderBy(q => q.Town);
		}

		public override IOrderedQueryable<PhoneHotline> ApplyOrder(IOrderedQueryable<PhoneHotline> query) {
			return query.ThenBy(q => q.Town);
		}
	}
}