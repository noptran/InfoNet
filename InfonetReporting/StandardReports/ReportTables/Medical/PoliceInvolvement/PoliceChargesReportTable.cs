using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.PoliceInvolvement {
	public class PoliceChargesReportTable : ReportTable<MedicalCJPoliceInvolvementPoliceChargeLineItem> {
		public PoliceChargesReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJPoliceInvolvementPoliceChargeLineItem item) {
			if (item.PoliceChargeType.HasValue && !Rows.Any(r => r.Code == item.PoliceChargeType)) {
				ReportRow newRow = new ReportRow { Code = item.PoliceChargeType, Title = Lookups.Statute[item.PoliceChargeType].Description, Counts = GetBlankDictionary(Headers) };
				foreach (var header in Headers)
					if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
						foreach (var subheader in header.SubHeaders)
							newRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
				Rows.Add(newRow);
			} else {
				foreach (ReportRow row in Rows)
					if (row.Code == item.PoliceChargeType)
						foreach (var header in Headers)
							if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
								foreach (var subheader in header.SubHeaders)
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
			}
		}
	}
}