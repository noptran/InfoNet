using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.PhoneHotlines {
	public class HotlineDateReportOrder : ReportOrder<PhoneHotline> {

		public override string ReportOrderAsString {
			get {
				return "Date";
			}
		}

		public override IOrderedQueryable<PhoneHotline> ApplyOrder(IQueryable<PhoneHotline> query) {
			return query.OrderBy(q => q.Date);
		}

		public override IOrderedQueryable<PhoneHotline> ApplyOrder(IOrderedQueryable<PhoneHotline> query) {
			return query.ThenBy(q => q.Date);
		}
	}
}