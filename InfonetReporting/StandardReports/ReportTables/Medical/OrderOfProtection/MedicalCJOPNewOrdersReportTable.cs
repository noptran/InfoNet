using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.OrderOfProtection {
	public class MedicalCJOPNewOrdersReportTable : ReportTable<MedicalCJOrderofProtectionLineItem> {
		public MedicalCJOPNewOrdersReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJOrderofProtectionLineItem item) {
			if (item.IsValidOrder && (item.IsNewFiled ?? false))
				foreach (var row in Rows) {
					foreach (ReportTableHeader header in Headers) {
						foreach (var subheader in header.SubHeaders)
							switch (header.Code) {
								case ReportTableHeaderEnum.Number:
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
									break;
								case ReportTableHeaderEnum.Percent:
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] = (int)ReportStringOutputEnum.NA;
									break;
							}
					}
				}
		}
	}
}