using System;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class OrderOfProtectionDateIssuedFilter : DateFilter {
		public OrderOfProtectionDateIssuedFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Date Issued";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.OrderOfProtection.Predicates.Add(OrderOfProtection.DateIssuedBetween(From, To));
		}
	}
}