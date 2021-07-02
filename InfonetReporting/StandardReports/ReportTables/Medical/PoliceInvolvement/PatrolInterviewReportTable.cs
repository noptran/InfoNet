using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.PoliceInvolvement {
	public class PatrolInterviewReportTable : ReportTable<MedicalCJPoliceInvolvementCJLineItem> {
		public PatrolInterviewReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJPoliceInvolvementCJLineItem item) {
			if (item.PatrolInterview)
				foreach (var row in Rows) {
					foreach (var header in Headers)
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders)
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
				}
		}
	}
}