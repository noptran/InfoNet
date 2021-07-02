using System.Collections.Generic;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class EthnicityReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<int?, HashSet<int?>>>> _uniqueClients = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<int?, HashSet<int?>>>>();

		public EthnicityReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void PreCheckAndApply(ReportContainer container) {
			foreach (var header in Headers) {
				var innerDict = new Dictionary<ReportTableSubHeaderEnum, Dictionary<int?, HashSet<int?>>>();
				foreach (var subheader in header.SubHeaders) {
					var rowDict = new Dictionary<int?, HashSet<int?>>();
					foreach (var row in Rows)
						rowDict.Add(row.Code, new HashSet<int?>());
					innerDict.Add(subheader.Code, rowDict);
				}
				_uniqueClients.Add(header.Code, innerDict);
			}
		}

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			foreach (var row in Rows.Where(r => r.Code == item.EthnicityID))
				foreach (var header in Headers) // Check New vs. Ongoing - allow Total
					if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
						foreach (var subheader in header.SubHeaders) {
							var currentSet = _uniqueClients[header.Code][subheader.Code][row.Code];
							// Check if Client Type matches
							if ((item.ClientTypeID == (int)subheader.Code || subheader.Code == ReportTableSubHeaderEnum.Total) && currentSet.Add(item.ClientID))
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
						}
		}
	}
}