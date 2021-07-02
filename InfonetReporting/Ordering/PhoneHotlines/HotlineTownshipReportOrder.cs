using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.PhoneHotlines {
	public class HotlineTownshipReportOrder : ReportOrder<PhoneHotline> {

		public override string ReportOrderAsString {
			get {
				return "Township";
			}
		}

		public override IOrderedQueryable<PhoneHotline> ApplyOrder(IQueryable<PhoneHotline> query) {
			return query.OrderBy(q => q.Township);
		}

		public override IOrderedQueryable<PhoneHotline> ApplyOrder(IOrderedQueryable<PhoneHotline> query) {
			return query.ThenBy(q => q.Township);
		}
	}
}