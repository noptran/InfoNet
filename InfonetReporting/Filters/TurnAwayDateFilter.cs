using System;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class TurnAwayDateFilter : DateFilter {
		public TurnAwayDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Turn Away Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.TurnAwayService.Predicates.Add(TurnAwayService.TurnAwayDateBetween(From, To));
		}
	}
}