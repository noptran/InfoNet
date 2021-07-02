using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.ServiceDetailsOfClient {
	public class ServiceDetailOfClientServiceNameReportOrder : ReportOrder<ServiceDetailOfClient> {
		public override string ReportOrderAsString { get { return "Service Name"; } }

		public override IOrderedQueryable<ServiceDetailOfClient> ApplyOrder(IOrderedQueryable<ServiceDetailOfClient> query) {
			return query.ThenBy(q => q.TLU_Codes_ProgramsAndServices.Description);
		}

		public override IOrderedQueryable<ServiceDetailOfClient> ApplyOrder(IQueryable<ServiceDetailOfClient> query) {
			return query.OrderBy(q => q.TLU_Codes_ProgramsAndServices.Description);
		}
	}
}