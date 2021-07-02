using System.Collections.Generic;
using Infonet.Core.Collections;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.VictimSensitiveInterviews {
	public class VictimSensitiveInterviewTotalCasesReportTable : ReportTable<VictimSensitiveInterviewLineItem> {
		public VictimSensitiveInterviewTotalCasesReportTable(string title, int displayOrder) : base(title, displayOrder) {
			UniqueClients = new Dictionary<string, Dictionary<string, HashSet<string>>>();
		}

		private bool ClientDictionaryInitialized { get; set; }

		private void InitializeClientDictionary() {
			UniqueClients = new Dictionary<string, Dictionary<string, HashSet<string>>>();
			foreach (var header in Headers) {
				var clientDict = new Dictionary<string, HashSet<string>>();
				foreach (var subheaader in header.SubHeaders)
					clientDict.Add(subheaader.Code.ToString(), new HashSet<string>());
				UniqueClients.Add(header.Code.ToString(), clientDict);
			}
			ClientDictionaryInitialized = true;
		}

		private Dictionary<string, Dictionary<string, HashSet<string>>> UniqueClients { get; set; }

		public override void CheckAndApply(VictimSensitiveInterviewLineItem item) {
			if (!ClientDictionaryInitialized)
				InitializeClientDictionary();
			string caseIdentifier = $"{item.ClientID}:{item.CaseID}";
			foreach (var row in Rows) {
				foreach (var header in Headers)
					if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
						foreach (var subheader in header.SubHeaders)
							if (subheader.Code.ToInt32() == item.ClientTypeID || subheader.Code == ReportTableSubHeaderEnum.Total) {
								UniqueClients[header.Code.ToString()][subheader.Code.ToString()].Add(caseIdentifier);
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] = UniqueClients[header.Code.ToString()][subheader.Code.ToString()].Count;
							}
			}
		}
	}
}