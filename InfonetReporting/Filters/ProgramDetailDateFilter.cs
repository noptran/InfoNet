using System;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ProgramDetailDateFilter : DateFilter {
		public ProgramDetailDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Program Detail Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ProgramDetail.Predicates.Add(ProgramDetail.PDateBetween(From, To));
		}
	}
}