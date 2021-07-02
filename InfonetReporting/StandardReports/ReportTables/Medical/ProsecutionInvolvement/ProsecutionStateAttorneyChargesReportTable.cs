using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.ProsecutionInvolvement {
	public class ProsecutionStateAttorneyChargesReportTable : ReportTable<MedicalCJProsecutionInvolvementTrialChargeLineItem> {
		public ProsecutionStateAttorneyChargesReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJProsecutionInvolvementTrialChargeLineItem item) {
			if (item.StatesAttorneyCharge.HasValue) {
				if (Rows.All(r => r.Code != item.StatesAttorneyCharge)) {
					var newRow = new ReportRow { Code = item.StatesAttorneyCharge, Title = Lookups.Statute[item.StatesAttorneyCharge]?.Description, Counts = GetBlankDictionary(Headers) };
					foreach (var header in Headers)
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
							newRow.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += 1;
					Rows.Add(newRow);
				} else {
					foreach (var row in Rows)
						if (item.StatesAttorneyCharge == row.Code)
							foreach (var header in Headers)
								if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
									row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += 1;
				}
			}
		}
	}
}