using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.ServiceDetailsOfClient {
	public class ServiceDetailOfClientCountyReportOrder : ReportOrder<ServiceDetailOfClient> {
		public override string ReportOrderAsString { get { return "County"; } }

		public override IOrderedQueryable<ServiceDetailOfClient> ApplyOrder(IOrderedQueryable<ServiceDetailOfClient> query) {
			return query.ThenBy(sd => sd.TwnTshipCounty.CountyID);
		}

		public override IOrderedQueryable<ServiceDetailOfClient> ApplyOrder(IQueryable<ServiceDetailOfClient> query) {
			return query.OrderBy(sd => sd.TwnTshipCounty.CountyID);
		}
	}
}