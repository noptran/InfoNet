using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.Medical {
	public class MedicalTotalVictimsReportTable : ReportTable<InvestigationMedicalLineItem> {
		public MedicalTotalVictimsReportTable(string title, int displayOrder) : base(title, displayOrder) {
			ClientCases = new Dictionary<ReportTableHeaderEnum, HashSet<string>>();
		}

		private Dictionary<ReportTableHeaderEnum, HashSet<string>> ClientCases { get; }

		public override void CheckAndApply(InvestigationMedicalLineItem item) {
			string caseIdentifier = $"{item.ClientID}:{item.CaseID}";
			foreach (var row in Rows) {
				foreach (var header in Headers)
					if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total) {
						HashSet<string> cases;
						bool exists = ClientCases.TryGetValue(header.Code, out cases);
						if (exists)
							cases.Add(caseIdentifier);
						else
							ClientCases.Add(header.Code, cases = new HashSet<string> { caseIdentifier });
						foreach (var subheader in header.SubHeaders)
							row.Counts[header.Code.ToString()][subheader.Code.ToString()] = cases.Count;
					}
			}
		}
	}
}