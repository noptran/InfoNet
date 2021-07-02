using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.Offender {
	public class MedicalCJOffendersTotalOffenderCasesReportTable : ReportTable<MedicalCJOffendersLineItem> {
		public MedicalCJOffendersTotalOffenderCasesReportTable(string title, int displayOrder) : base(title, displayOrder) {
			OffenderCases = new HashSet<string>();
		}

		private HashSet<string> OffenderCases { get; }

		public override void CheckAndApply(MedicalCJOffendersLineItem item) {
			string offenserCaseIdentifier = $"{item.OffenderID}:{item.ClientID}:{item.CaseID}";
			if (!OffenderCases.Contains(offenserCaseIdentifier))
				foreach (var row in Rows) {
					foreach (var header in Headers)
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders) {
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
								OffenderCases.Add(offenserCaseIdentifier);
							}
				}
		}
	}
}