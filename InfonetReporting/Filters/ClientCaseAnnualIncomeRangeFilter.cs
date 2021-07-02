using System.IO;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using LinqKit;

namespace Infonet.Reporting.Filters {
	public class ClientCaseAnnualIncomeRangeFilter : ReportFilter {
		public ClientCaseAnnualIncomeRangeFilter(decimal[] rangesLowerBounds, decimal?[] rangesUpperBounds) {
			Visible = false;
			RangesLowerBounds = rangesLowerBounds;
			RangesUpperBounds = rangesUpperBounds;
		}

		public decimal[] RangesLowerBounds { get; set; }

		public decimal?[] RangesUpperBounds { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var predicate = PredicateBuilder.New<ClientCase>(false);
			for (int x = 0; x < RangesLowerBounds.Length; x++)
				predicate = predicate.Or(ClientCase.TotalAnnualIncomeBetween(RangesLowerBounds[x], RangesUpperBounds[x]));
			context.ClientCase.Predicates.Add(predicate);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) { }
	}
}