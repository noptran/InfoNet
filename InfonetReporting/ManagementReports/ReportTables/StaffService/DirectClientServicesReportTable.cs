using System.Collections.Generic;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.StaffService {
	public class DirectClientServicesReportTable : ReportTable<ProgramsAndServicesDirectClientServicesLineItem> {
		private readonly Dictionary<int?, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _clientIdsByType = new Dictionary<int?, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();
		private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _uniqueClientsByType = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();

		public DirectClientServicesReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void PreCheckAndApply(ReportContainer container) {
			foreach (var header in Headers) {
				var innerDict = new Dictionary<ReportTableSubHeaderEnum, HashSet<int>>();
				foreach (var subheader in header.SubHeaders)
					innerDict.Add(subheader.Code, new HashSet<int>());
				_uniqueClientsByType.Add(header.Code, innerDict);
			}
			foreach (var row in Rows) {
				var innerDict = new Dictionary<ReportTableSubHeaderEnum, HashSet<int>>();
				foreach (var subheader in Headers.First().SubHeaders)
					innerDict.Add(subheader.Code, new HashSet<int>());
				_clientIdsByType.Add(row.Code, innerDict);
			}
		}

		public override void CheckAndApply(ProgramsAndServicesDirectClientServicesLineItem item) {
			foreach (var row in Rows.Where(r => r.Code == item.ServiceID)) {
				foreach (var header in Headers) {
					foreach (var subheader in header.SubHeaders)
						switch (header.Code) {
							case ReportTableHeaderEnum.NumberOfClients:
								_clientIdsByType[row.Code][subheader.Code].Add(item.ClientID.Value);
								_uniqueClientsByType[header.Code][subheader.Code].Add(item.ClientID.Value);
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] = _clientIdsByType[row.Code][subheader.Code].Count;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] = _uniqueClientsByType[header.Code][subheader.Code].Count;
								break;
							case ReportTableHeaderEnum.Hours:
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.ReceivedHours ?? 0.0;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.ReceivedHours ?? 0.0;
								break;
						}
				}
			}
		}
	}
}