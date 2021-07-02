using System;
using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.OrderOfProtection {
	public class MedicalCJOPTypeOfUpgradeReportTable : ReportTable<MedicalCJOrderofProtectionLineItem> {
		private double _divisor = 0.0;
		private readonly HashSet<string> _ordersAndActivities = new HashSet<string>();

		public MedicalCJOPTypeOfUpgradeReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJOrderofProtectionLineItem item) {
			if (item.HasOpActivityInRange && item.IsValidOrder) {
				_divisor += item.NumberOfUpgrades;
				foreach (ReportRow row in Rows) {
					foreach (var activity in item.ValidUpgradedOpActivities) {
						string orderActivityIdentifier = $"{item.OP_ID}:{activity.OpActivityCodeID}";
						if (!_ordersAndActivities.Contains(orderActivityIdentifier))
							if (activity.OpActivityCodeID == row.Code)
								foreach (ReportTableHeader header in Headers) {
									foreach (ReportTableSubHeader subheader in header.SubHeaders)
										switch (header.Code) {
											case ReportTableHeaderEnum.Number:
												row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
												_ordersAndActivities.Add(orderActivityIdentifier);
												break;
										}
								}
					}
				}
				foreach (ReportRow row in Rows)
					row.Counts[ReportTableHeaderEnum.Percent.ToString()][ReportTableSubHeaderEnum.Total.ToString()] = Math.Round(row.Counts[ReportTableHeaderEnum.Number.ToString()][ReportTableSubHeaderEnum.Total.ToString()] / _divisor * 100, 1);
			}
		}
	}
}