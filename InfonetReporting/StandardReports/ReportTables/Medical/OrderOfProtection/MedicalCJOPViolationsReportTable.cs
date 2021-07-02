using System;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.OrderOfProtection {
	public class MedicalCJOPViolationsReportTable : ReportTable<MedicalCJOrderofProtectionLineItem> {
		public MedicalCJOPViolationsReportTable(string title, int displayOrder) : base(title, displayOrder) {
			_divisor = 0.0;
		}

		private double _divisor;
		public DateTime? StartDate;
		public DateTime? EndDate;

		public override void CheckAndApply(MedicalCJOrderofProtectionLineItem item) {
			if (item.IsValidOrder && item.IsActiveOrder) {
				_divisor += 1;
				foreach (var row in Rows) {
					foreach (var header in Headers) {
						foreach (var subheader in header.SubHeaders)
							if (header.Code == ReportTableHeaderEnum.Number)
								switch (row.Code) {
									case (int)Violations.NoViolation:
										if (!item.OpActivities.Any(opa => opa.OpActivityDate.HasValue && opa.OpActivityDate.Value >= StartDate && opa.OpActivityDate.Value <= EndDate && (opa.OpActivityCodeID == 5 || opa.OpActivityCodeID == 7)))
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										break;
									case (int)Violations.ViolationWithoutPoliceCharge:
										if (item.OpActivities.Any(opa => opa.OpActivityDate.HasValue && opa.OpActivityDate.Value >= StartDate && opa.OpActivityDate.Value <= EndDate && opa.OpActivityCodeID == 5))
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										break;
									case (int)Violations.ViolationWithPoliceCharge:
										if (item.OpActivities.Any(opa => opa.OpActivityDate.HasValue && opa.OpActivityDate.Value >= StartDate && opa.OpActivityDate.Value <= EndDate && opa.OpActivityCodeID == 7))
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										break;
								}
							else
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] = Math.Round(row.Counts[ReportTableHeaderEnum.Number.ToString()][ReportTableSubHeaderEnum.Total.ToString()] / _divisor * 100, 1);
					}
				}
			}
		}
	}

	internal enum Violations {
		NoViolation,
		ViolationWithoutPoliceCharge,
		ViolationWithPoliceCharge
	}
}