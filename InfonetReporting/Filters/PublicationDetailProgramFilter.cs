using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class PublicationDetailProgramFilter : LookupFilter {
		public PublicationDetailProgramFilter(int?[] codeIds = null) : base(Lookups.ProgramsAndServices, codeIds) {
			Label = "Publication Type";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PublicationDetail.Predicates.Add(q => CodeIds.Contains(q.ProgramID));
		}
	}
}