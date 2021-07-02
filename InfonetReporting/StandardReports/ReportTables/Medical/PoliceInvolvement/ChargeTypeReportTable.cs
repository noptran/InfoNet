using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.PoliceInvolvement {
	public class ChargeTypeReportTable : ReportTable<MedicalCJPoliceInvolvementPoliceChargeLineItem> {
		public ChargeTypeReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJPoliceInvolvementPoliceChargeLineItem item) {
			foreach (var row in Rows)
				if (item.SuspectChargeType == row.Code)
					foreach (var header in Headers)
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders)
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
		}
	}
}