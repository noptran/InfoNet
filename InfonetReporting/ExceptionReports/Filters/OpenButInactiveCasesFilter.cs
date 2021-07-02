using System;
using System.IO;
using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.ExceptionReports.Filters {
	public class OpenButInactiveCasesFilter : ReportFilter {
		public OpenButInactiveCasesFilter(int days) {
			Label = "Days Since Last Service Exceed";
			Days = days;
		}

		public int Days { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			var threshold = DateTime.Today - new TimeSpan(Days, 0, 0);
			context.ClientCase.Predicates.Add(cc =>
				(cc.CaseClosed == null || cc.CaseClosed == 0) &&
				!cc.ServiceDetailsOfClient.Any(sd =>
					ServiceDetailOfClient.AllShelterIds.Contains(sd.ServiceID)
						? sd.ShelterEndDate == null || sd.ShelterEndDate >= threshold
						: sd.ServiceDate >= threshold)
			);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.Write(Days);
		}
	}
}