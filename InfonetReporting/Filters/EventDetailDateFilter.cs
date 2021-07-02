using System;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class EventDetailDateFilter : DateFilter {
		public EventDetailDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Event Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.EventDetail.Predicates.Add(EventDetail.EventDateBetween(From, To));
		}
	}
}