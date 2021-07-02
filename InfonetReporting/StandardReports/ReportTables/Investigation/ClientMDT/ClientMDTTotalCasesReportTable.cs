using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.ClientMDT {
	public class ClientMDTTotalCasesReportTable : ReportTable<ClientMDTLineItem> {
		public ClientMDTTotalCasesReportTable(string title, int displayOrder) : base(title, displayOrder) {
			UniqueCases = new Dictionary<ReportTableHeaderEnum, HashSet<string>>();
		}

		private Dictionary<ReportTableHeaderEnum, HashSet<string>> UniqueCases { get; set; }

		public override void CheckAndApply(ClientMDTLineItem item) {
			foreach (var row in Rows) {
				foreach (var header in Headers) // Check New vs. Ongoing - allow Total
					if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
						foreach (var subheader in header.SubHeaders) {
							string itemKey = $"{item.ClientId}:{item.CaseId}";
							HashSet<string> output;
							if (UniqueCases.TryGetValue(header.Code, out output))
								output.Add(itemKey);
							else
								UniqueCases.Add(header.Code, new HashSet<string> { itemKey });
							row.Counts[header.Code.ToString()][subheader.Code.ToString()] = UniqueCases[header.Code].Count;
						}
			}
		}
	}
}