using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.Client {
	public class ClientChildrenWithBehavioralIssuesCountReportTable : ReportTable<ClientChildBehavioralIssuesLineItem> {
		public ClientChildrenWithBehavioralIssuesCountReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientChildBehavioralIssuesLineItem item) {
			foreach (var row in Rows) {
				foreach (var header in Headers)
					if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
						foreach (var subheader in header.SubHeaders)
							row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
			}
		}
	}
}