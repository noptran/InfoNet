using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class HotlineCallTypeFilter : LookupFilter {
		public HotlineCallTypeFilter(int?[] codeIds = null) : base(Lookups.HotlineCallType, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PhoneHotline.Predicates.Add(q => CodeIds.Contains(q.CallTypeID));
		}
	}
}