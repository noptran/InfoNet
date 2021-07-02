using System.Collections.Generic;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.DirectServices {
	public class ShelterServiceReportTable : ReportTable<DirectServiceLineItem> {
		private readonly Dictionary<int?, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _clientIdsByType = new Dictionary<int?, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();
		private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _uniqueClientsByType = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();

		public ShelterServiceReportTable(string title, int displayOrder) : base(title, displayOrder) { }

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

		public override void CheckAndApply(DirectServiceLineItem item) {
			foreach (var row in Rows.Where(r => r.Code == item.ServiceId))
				foreach (var eachHeader in Headers) {
					foreach (var eachClientType in eachHeader.SubHeaders.Where(ct => (int)ct.Code == item.ClientTypeId || ct.Code == ReportTableSubHeaderEnum.Total))
						switch (eachHeader.Code) {
							case ReportTableHeaderEnum.NumberOfClientsReceivingShelter:
								var clientIdsForRowAndType = _clientIdsByType[row.Code][eachClientType.Code];
								clientIdsForRowAndType.Add(item.ClientId.Value);
								row.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] = clientIdsForRowAndType.Count;

								var uniqueClientsForHeaderAndType = _uniqueClientsByType[eachHeader.Code][eachClientType.Code];
								uniqueClientsForHeaderAndType.Add(item.ClientId.Value);
								NonDuplicatedSubtotalRow.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] = uniqueClientsForHeaderAndType.Count;
								break;
							case ReportTableHeaderEnum.DaysOfShelterReceived:
								row.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] += item.DaysOfShelter ?? 0;
								NonDuplicatedSubtotalRow.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] += item.DaysOfShelter ?? 0;
								break;
						}
				}
		}
	}
}