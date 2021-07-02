using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.ProgramDetailsStaff {
	public class ProgramDetailDateReportOrder : ReportOrder<ProgramDetailStaff> {

		public override string ReportOrderAsString {
			get {
				return "Date";
			}
		}

		public override IOrderedQueryable<ProgramDetailStaff> ApplyOrder(IQueryable<ProgramDetailStaff> query) {
			return query.OrderBy(q => q.ProgramDetail.PDate);
		}

		public override IOrderedQueryable<ProgramDetailStaff> ApplyOrder(IOrderedQueryable<ProgramDetailStaff> query) {
			return query.ThenBy(q => q.ProgramDetail.PDate);
		}
	}
}