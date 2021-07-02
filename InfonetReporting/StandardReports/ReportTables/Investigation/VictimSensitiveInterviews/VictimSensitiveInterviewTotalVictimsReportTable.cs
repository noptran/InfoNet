using System.Collections.Generic;
using Infonet.Core.Collections;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.VictimSensitiveInterviews {

	public class VictimSensitiveInterviewTotalVictimsReportTable : ReportTable<VictimSensitiveInterviewLineItem> {
		public VictimSensitiveInterviewTotalVictimsReportTable(string title, int displayOrder) : base(title, displayOrder) {		
		}

		private bool ClientDictionaryInitialized { get; set; }
		private void InitializeClientDictionary() {
			UniqueClients = new Dictionary<string, Dictionary<string, HashSet<int?>>>();
			foreach (ReportTableHeader header in Headers) {
				Dictionary<string, HashSet<int?>> clientDict = new Dictionary<string, HashSet<int?>>();
				foreach (ReportTableSubHeader subheaader in header.SubHeaders) {
					clientDict.Add(subheaader.Code.ToString(), new HashSet<int?>());
				}
				UniqueClients.Add(header.Code.ToString(), clientDict);
			}
			ClientDictionaryInitialized = true;
		}

		private Dictionary<string, Dictionary<string, HashSet<int?>>> UniqueClients { get; set; }

		public override void CheckAndApply(VictimSensitiveInterviewLineItem item) {
			if (!ClientDictionaryInitialized) {
				InitializeClientDictionary();
			}
			foreach (ReportRow row in Rows) {
				foreach (ReportTableHeader header in Headers) {
					// Check New vs. Ongoing - allow Total
					if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total) {
						foreach (ReportTableSubHeader subheader in header.SubHeaders) {
							if (subheader.Code.ToInt32() == item.ClientTypeID || subheader.Code == ReportTableSubHeaderEnum.Total) {
								HashSet<int?> currentList = UniqueClients[header.Code.ToString()][subheader.Code.ToString()];
								// Check if Client Type matches
								if (currentList.Add(item.ClientID)) {
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] = currentList.Count;
								}
							}
						}
					}
				}
			}
		}


	}
}