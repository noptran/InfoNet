using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.ProgramDetailsStaff {
	public class ProgramDetailServiceReportOrder : ReportOrder<ProgramDetailStaff> {

		public override string ReportOrderAsString {
			get {
				return "Service Name";
			}
		}

		public override IOrderedQueryable<ProgramDetailStaff> ApplyOrder(IQueryable<ProgramDetailStaff> query) {
			return query.OrderBy(q => q.ProgramDetail.TLU_Codes_ProgramsAndServices.Description);
		}

		public override IOrderedQueryable<ProgramDetailStaff> ApplyOrder(IOrderedQueryable<ProgramDetailStaff> query) {
			return query.ThenBy(q => q.ProgramDetail.TLU_Codes_ProgramsAndServices.Description);
		}
	}
}
