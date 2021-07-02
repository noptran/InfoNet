using System;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ServiceOutcomeDateFilter : DateFilter {
		public ServiceOutcomeDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Outcome Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ServiceOutcome.Predicates.Add(ServiceOutcome.OutcomeDateBetween(From, To));
		}
	}
}