using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	/* Assumes Lookups.Sex is a subset of Lookups.GenderIdentity.  Uses GenderIdentity because those
	   codes are selectable in the Standard Report UI even though not allowed in the database. */
	public class HotlineSexFilter : LookupFilter {
		public HotlineSexFilter(int?[] codeIds = null) : base(Lookups.GenderIdentity, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PhoneHotline.Predicates.Add(ph => CodeIds.Contains(ph.SexID));
		}
	}
}