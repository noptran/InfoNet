using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.OrderOfProtection {
	public class MedicalCJOPOrderUpgradesMadeReportTable : ReportTable<MedicalCJOrderofProtectionLineItem> {

		public MedicalCJOPOrderUpgradesMadeReportTable(string title, int displayOrder) : base(title, displayOrder) {
		}

		public override void CheckAndApply(MedicalCJOrderofProtectionLineItem item) {
			if (item.HasOpActivityInRange && item.IsValidOrder) {
				foreach (ReportRow row in Rows) {
					foreach (ReportTableHeader header in Headers) {
						foreach (ReportTableSubHeader subheader in header.SubHeaders) {
							switch (header.Code) {
								case ReportTableHeaderEnum.Number:
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.NumberOfUpgrades;
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
	}


	internal enum OpActivityId {
		EmergencyToInterim = 1,
		EmergencyToPlenary = 2,
		InterimToPlenary = 3
	}
}