using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ProgramDetailProgramFilter : LookupFilter {
		public ProgramDetailProgramFilter(int?[] codeIds = null) : base(Lookups.ProgramsAndServices, codeIds) {
			Label = "Program";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ProgramDetail.Predicates.Add(pd => CodeIds.Contains(pd.ProgramID));
		}
	}
}