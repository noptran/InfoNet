using System.Data.Entity;
using System.IO;
using System.Linq;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.ExceptionReports.Filters {
	public class OpenAndLengthyShelterStaysFilter : ReportFilter {
		public OpenAndLengthyShelterStaysFilter(int days) {
			Label = "Shelter Days Exceed";
			Days = days;
		}

		public int Days { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ServiceDetailOfClient.Predicates.Add(
				q => ServiceDetailOfClient.AllShelterIds.Contains(q.ServiceID) &&
					(
						q.ShelterEndDate == null ||
						q.ShelterBegDate > q.ShelterEndDate ||
						DbFunctions.DiffDays(q.ShelterBegDate.Value, q.ShelterEndDate) + 1 > Days
					)
			);
		}

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.Write(Days);
		}
	}
}