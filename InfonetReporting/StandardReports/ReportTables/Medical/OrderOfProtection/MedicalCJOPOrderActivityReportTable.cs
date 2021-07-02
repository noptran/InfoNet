using System;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.OrderOfProtection {
	public class MedicalCJOPOrderActivityReportTable : ReportTable<MedicalCJOrderofProtectionLineItem> {
		private double _divisor = 0.0;

		public MedicalCJOPOrderActivityReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public DateTime? StartDate;

		public DateTime? EndDate;

		public override void CheckAndApply(MedicalCJOrderofProtectionLineItem item) {
			if (item.IsActiveOrder && item.IsValidOrder)
				_divisor += 1;
			if (item.IsValidOrder)
				foreach (ReportRow row in Rows) {
					foreach (ReportTableHeader header in Headers) {
						foreach (ReportTableSubHeader subheader in header.SubHeaders)
							if (header.Code == ReportTableHeaderEnum.Number)
								switch (row.Code) {
									case (int)OPOrderActivity.Expired:
										DateTime? dateToUse = item.MaxNewExpirationDate ?? item.OriginalExpirationDate;
										if (dateToUse.HasValue && dateToUse >= StartDate && dateToUse <= EndDate)
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										break;
									case (int)OPOrderActivity.Vacated:
										if (item.DateVacated.HasValue && item.DateVacated >= StartDate && item.DateVacated <= EndDate)
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										break;
									case (int)OPOrderActivity.Extended:
										if (item.OpActivities.Any(a => a.OpActivityDate.HasValue && a.OpActivityDate >= StartDate && a.OpActivityDate <= EndDate && a.OpActivityCodeID == 6))
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										break;
									case (int)OPOrderActivity.Modified:
										if (item.OpActivities.Any(a => a.OpActivityDate.HasValue && a.OpActivityDate >= StartDate && a.OpActivityDate <= EndDate && a.OpActivityCodeID == 4))
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										break;
								}
							else
								row.Counts[ReportTableHeaderEnum.Percent.ToString()][ReportTableSubHeaderEnum.Total.ToString()] = Math.Round(row.Counts[ReportTableHeaderEnum.Number.ToString()][ReportTableSubHeaderEnum.Total.ToString()] / _divisor * 100, 1);
					}
				}
		}
	}

	internal enum OPOrderActivity {
		Expired,
		Vacated,
		Extended,
		Modified
	}
}