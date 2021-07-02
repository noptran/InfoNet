using System;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class OtherStaffActivityDateFilter : DateFilter {
		public OtherStaffActivityDateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Activity Date";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.OtherStaffActivity.Predicates.Add(OtherStaffActivity.OsaDateBetween(From, To));
		}
	}
}