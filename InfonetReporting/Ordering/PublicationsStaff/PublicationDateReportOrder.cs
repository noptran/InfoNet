using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.PublicationsStaff {
	public class PublicationDateReportOrder : ReportOrder<PublicationDetailStaff> {

		public override string ReportOrderAsString {
			get {
				return "Date";
			}
		}

		public override IOrderedQueryable<PublicationDetailStaff> ApplyOrder(IQueryable<PublicationDetailStaff> query) {
			return query.OrderBy(q => q.PublicationDetail.PDate);
		}

		public override IOrderedQueryable<PublicationDetailStaff> ApplyOrder(IOrderedQueryable<PublicationDetailStaff> query) {
			return query.ThenBy(q => q.PublicationDetail.PDate);
		}
	}
}