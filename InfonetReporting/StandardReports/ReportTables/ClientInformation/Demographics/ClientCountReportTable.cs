using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class ClientCountReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public ClientCountReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		private bool ClientDictionaryInitialized { get; set; }

		private void InitializeClientDictionary() {
			UniqueClients = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int?>>>();
			foreach (var header in Headers) {
				var clientDict = new Dictionary<ReportTableSubHeaderEnum, HashSet<int?>>();
				foreach (var subheaader in header.SubHeaders)
					clientDict.Add(subheaader.Code, new HashSet<int?>());
				UniqueClients.Add(header.Code, clientDict);
			}
			ClientDictionaryInitialized = true;
		}

		private Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int?>>> UniqueClients { get; set; }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			if (!ClientDictionaryInitialized)
				InitializeClientDictionary();
			foreach (var row in Rows) {
				foreach (var newOrOngoing in Headers) // Check New vs. Ongoing - allow Total
					if (item.ClientStatus == newOrOngoing.Code || newOrOngoing.Code == ReportTableHeaderEnum.Total)
						foreach (var clientType in newOrOngoing.SubHeaders) {
							var currentList = UniqueClients[newOrOngoing.Code][clientType.Code];
							// Check if Client Type matches
							if ((item.ClientTypeID == (int)clientType.Code || clientType.Code == ReportTableSubHeaderEnum.Total) && currentList.Add(item.ClientID))
								row.Counts[newOrOngoing.Code.ToString()][clientType.Code.ToString()] = currentList.Count;
						}
			}
		}
	}
}