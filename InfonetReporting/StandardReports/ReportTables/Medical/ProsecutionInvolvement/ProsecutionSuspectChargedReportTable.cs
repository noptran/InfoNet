using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.ProsecutionInvolvement {
	public class ProsecutionSuspectChargedReportTable : ReportTable<MedicalCJProsecutionInvolvementTrialChargeLineItem> {
		private readonly HashSet<int?> _offenderIds = new HashSet<int?>();

		public ProsecutionSuspectChargedReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJProsecutionInvolvementTrialChargeLineItem item) {
			if (item.SuspectCharged && !_offenderIds.Contains(item.OffenderId))
				foreach (var row in Rows) {
					foreach (var header in Headers)
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total) {
							row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += 1;
							_offenderIds.Add(item.OffenderId);
						}
				}
		}
	}
}