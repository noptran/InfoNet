using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.OrderOfProtection {
	public class MedicalCJOPActiveOrdersReportTable : ReportTable<MedicalCJOrderofProtectionLineItem> {

		public MedicalCJOPActiveOrdersReportTable(string title, int displayOrder) : base(title, displayOrder) {
		}

		public override void CheckAndApply(MedicalCJOrderofProtectionLineItem item) {
			if (item.IsActiveOrder && item.IsValidOrder) {
				foreach (ReportRow row in Rows) {
					foreach (ReportTableHeader header in Headers) {
						foreach (ReportTableSubHeader subheader in header.SubHeaders) {
							switch (header.Code) {
								case ReportTableHeaderEnum.Percent:
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] = (int)ReportStringOutputEnum.NA;
									break;
								case ReportTableHeaderEnum.Number:
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
									break;
							}
						}
					}
				}
			}
		}
	}
}