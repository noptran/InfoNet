using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.ClientMDT {
	public class ClientMDTPositionReportTable : ReportTable<ClientMDTLineItem> {
		public ClientMDTPositionReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientMDTLineItem item) {
			foreach (var row in Rows)
				if (row.Code == item.PositionId)
					foreach (var header in Headers)
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders)
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
		}
	}
}