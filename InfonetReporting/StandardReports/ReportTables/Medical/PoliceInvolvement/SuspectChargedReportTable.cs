using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.PoliceInvolvement {
	public class SuspectChargedReportTable : ReportTable<MedicalCJPoliceInvolvementPoliceChargeLineItem> {
		private readonly HashSet<int?> _offenderIds = new HashSet<int?>();

		public SuspectChargedReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJPoliceInvolvementPoliceChargeLineItem item) {
			if (!_offenderIds.Contains(item.OffenderId))
				if (item.SuspectCharged)
					foreach (var row in Rows) {
						foreach (var header in Headers)
							if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
								foreach (var subheader in header.SubHeaders) {
									_offenderIds.Add(item.OffenderId);
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
								}
					}
		}
	}
}