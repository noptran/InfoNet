using System;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.OrderOfProtection {
	public class MedicalCJOPStatusOfOrdersReportTable : ReportTable<MedicalCJOrderofProtectionLineItem> {
		private double _divisor = 0.0;

		public MedicalCJOPStatusOfOrdersReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJOrderofProtectionLineItem item) {
			if (item.IsValidOrder && (item.IsNewFiled ?? false)) {
				_divisor += 1;
				foreach (ReportRow row in Rows)
					if (row.Code == item.StatusID)
						foreach (ReportTableHeader header in Headers) {
							foreach (ReportTableSubHeader subheader in header.SubHeaders)
								switch (header.Code) {
									case ReportTableHeaderEnum.Number:
										row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										break;
								}
						}

				foreach (ReportRow row in Rows)
					row.Counts[ReportTableHeaderEnum.Percent.ToString()][ReportTableSubHeaderEnum.Total.ToString()] = Math.Round(row.Counts[ReportTableHeaderEnum.Number.ToString()][ReportTableSubHeaderEnum.Total.ToString()] / _divisor * 100, 1);
			}
		}
	}
}