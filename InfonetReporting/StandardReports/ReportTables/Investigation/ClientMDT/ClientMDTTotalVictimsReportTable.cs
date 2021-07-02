using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.ClientMDT {

	public class ClientMDTTotalVictimsReportTable : ReportTable<ClientMDTLineItem> {
		public ClientMDTTotalVictimsReportTable(string title, int displayOrder) : base(title, displayOrder) {		
		}

		private bool ClientDictionaryInitialized { get; set; }
		private void InitializeClientDictionary() {
			UniqueClients = new Dictionary<string, Dictionary<string, HashSet<int?>>>();
			foreach (var header in Headers) {
				var clientDict = new Dictionary<string, HashSet<int?>>();
				foreach (var subheaader in header.SubHeaders) {
					clientDict.Add(subheaader.Code.ToString(), new HashSet<int?>());
				}
				UniqueClients.Add(header.Code.ToString(), clientDict);
			}
			ClientDictionaryInitialized = true;
		}

		private Dictionary<string, Dictionary<string, HashSet<int?>>> UniqueClients { get; set; }

		public override void CheckAndApply(ClientMDTLineItem item) {
			if (!ClientDictionaryInitialized) {
				InitializeClientDictionary();
			}
			foreach (var row in Rows) {
				foreach (var header in Headers) {
					// Check New vs. Ongoing - allow Total
					if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total) {
						foreach (var subheader in header.SubHeaders) {
							var currentList = UniqueClients[header.Code.ToString()][subheader.Code.ToString()];
							// Check if Client Type matches
							if (currentList.Add(item.ClientId)) {
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] = currentList.Count;
							}
						}
					}
				}
			}
		}


	}
}