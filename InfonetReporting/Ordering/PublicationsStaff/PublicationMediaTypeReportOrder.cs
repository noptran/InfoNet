using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.PublicationsStaff {
	public class PublicationMediaTypeReportOrder : ReportOrder<PublicationDetailStaff> {

		public override string ReportOrderAsString {
			get {
				return "Media Type";
			}
		}

		public override IOrderedQueryable<PublicationDetailStaff> ApplyOrder(IQueryable<PublicationDetailStaff> query) {
			return query.OrderBy(q => q.PublicationDetail.TLU_Codes_ProgramsAndServices.Description);
		}

		public override IOrderedQueryable<PublicationDetailStaff> ApplyOrder(IOrderedQueryable<PublicationDetailStaff> query) {
			return query.ThenBy(q => q.PublicationDetail.TLU_Codes_ProgramsAndServices.Description);
		}
	}
}
