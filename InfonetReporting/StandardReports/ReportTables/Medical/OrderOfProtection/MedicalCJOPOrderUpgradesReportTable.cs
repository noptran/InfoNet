using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.OrderOfProtection {
	public class MedicalCJOPOrderUpgradesReportTable : ReportTable<MedicalCJOrderofProtectionLineItem> {
		private readonly HashSet<int?> _opIds;

		public MedicalCJOPOrderUpgradesReportTable(string title, int displayOrder) : base(title, displayOrder) {
			_opIds = new HashSet<int?>();
		}

		public override void CheckAndApply(MedicalCJOrderofProtectionLineItem item) {
			if (item.HasOpActivityInRange && item.IsValidOrder) {
				if(!_opIds.Contains(item.OP_ID))
					foreach (ReportRow row in Rows) {
						foreach (ReportTableHeader header in Headers) {
							foreach (ReportTableSubHeader subheader in header.SubHeaders) {
								switch (header.Code) {
									case ReportTableHeaderEnum.Percent:
										row.Counts[header.Code.ToString()][subheader.Code.ToString()] = (int)ReportStringOutputEnum.NA;
										break;
									case ReportTableHeaderEnum.Number:
										row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										_opIds.Add(item.OP_ID);
										break;
								}
							}
						}
					}
				}
			}
		}
	
}