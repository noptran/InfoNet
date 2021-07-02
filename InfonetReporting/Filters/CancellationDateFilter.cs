using System;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class CancellationDateFilter : DateFilter {
		public CancellationDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Cancellation Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.Cancellation.Predicates.Add(Cancellation.DateBetween(From, To));
		}
	}
}