using System;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class HivMentalSubstanceDateFilter : DateFilter {
		public HivMentalSubstanceDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Aggregate Client Information Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.HivMentalSubstance.Predicates.Add(HivMentalSubstance.HMSDateBetween(From, To));
		}
	}
}