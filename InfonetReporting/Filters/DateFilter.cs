using System;

namespace Infonet.Reporting.Filters {
	public abstract class DateFilter : RangeFilter<DateTime> {
		protected DateFilter(DateTime? from, DateTime? to) : base(from, to) {
			Label = "Date Range";
			Format = "{0:d}";
		}
	}
}