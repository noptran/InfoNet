using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.StaffService {
	public class ClientCountReportTable : ReportTable<ManagementClientInformationDemographicsLineItem> {
		private readonly HashSet<int?> _clientIds = new HashSet<int?>();

		public ClientCountReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ManagementClientInformationDemographicsLineItem item) {
			if (!_clientIds.Contains(item.ClientID))
				foreach (var row in Rows) {
					foreach (var header in Headers)
						if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders) {
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
								_clientIds.Add(item.ClientID);
							}
				}
		}
	}
}