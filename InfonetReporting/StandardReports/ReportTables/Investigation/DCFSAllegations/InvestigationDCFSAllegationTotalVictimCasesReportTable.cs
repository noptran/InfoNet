using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.DCFSAllegations {
	public class InvestigationDCFSAllegationTotalVictimCasesReportTable : ReportTable<InvestigationDCFSAllegationLineItem> {
		public InvestigationDCFSAllegationTotalVictimCasesReportTable(string title, int displayOrder) : base(title, displayOrder) {
			UniqueCases = new Dictionary<ReportTableHeaderEnum, HashSet<string>>();
		}

		private Dictionary<ReportTableHeaderEnum, HashSet<string>> UniqueCases { get; }

		public override void CheckAndApply(InvestigationDCFSAllegationLineItem item) {
			string itemKey = $"{item.ClientId}:{item.CaseId}";
			foreach (var row in Rows) {
				foreach (var header in Headers)
					if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
						foreach (var subheader in header.SubHeaders) {
							HashSet<string> output;
							bool exists = UniqueCases.TryGetValue(header.Code, out output);
							if (exists) {
								if (!output.Contains(itemKey))
									UniqueCases[header.Code].Add(itemKey);
							} else {
								var set = new HashSet<string>();
								set.Add(itemKey);
								UniqueCases.Add(header.Code, set);
							}
							row.Counts[header.Code.ToString()][subheader.Code.ToString()] = UniqueCases[header.Code].Count;
						}
			}
		}
	}
}