using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.ServiceDetailsOfClient {
	public class ServiceDetailOfClientZipCodeReportOrder : ReportOrder<ServiceDetailOfClient> {
		public override string ReportOrderAsString { get { return "Zip Code"; } }

		public override IOrderedQueryable<ServiceDetailOfClient> ApplyOrder(IOrderedQueryable<ServiceDetailOfClient> query) {
			return query.ThenBy(sd => sd.TwnTshipCounty.Zipcode.Trim());
		}

		public override IOrderedQueryable<ServiceDetailOfClient> ApplyOrder(IQueryable<ServiceDetailOfClient> query) {
			return query.OrderBy(sd => sd.TwnTshipCounty.Zipcode.Trim());
		}
	}
}