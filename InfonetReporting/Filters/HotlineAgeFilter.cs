using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class HotlineAgeFilter : RangeFilter<int> {
		public HotlineAgeFilter(int? from, int? to) : base(from, to) {
			Label = "Age Range";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PhoneHotline.Predicates.Add(ph => ph.Age >= From && ph.Age <= To);
		}
	}
}