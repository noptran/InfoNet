using System.Linq;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Ordering.OrdersOfProtection {
	public class OrderOfProtectionClientCodeReportOrder : ReportOrder<OrderOfProtection> {

		public override string ReportOrderAsString {
			get {
				return "Client Code";
			}
		}

		public override IOrderedQueryable<OrderOfProtection> ApplyOrder(IQueryable<OrderOfProtection> query) {
			return query.OrderBy(sd => sd.ClientCase.Client.ClientCode);
		}

		public override IOrderedQueryable<OrderOfProtection> ApplyOrder(IOrderedQueryable<OrderOfProtection> query) {
			return query.ThenBy(sd => sd.ClientCase.Client.ClientCode);
		}
	}
}