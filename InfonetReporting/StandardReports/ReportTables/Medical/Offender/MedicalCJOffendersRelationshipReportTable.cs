using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.Offender {
	public class MedicalCJOffendersRelationshipReportTable : ReportTable<MedicalCJOffendersLineItem> {
		public MedicalCJOffendersRelationshipReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(MedicalCJOffendersLineItem item) {
			if (Rows.All(r => r.Code != item.RelationshipToVictimID)) {
				var newRow = new ReportRow { Code = item.RelationshipToVictimID, Title = Lookups.RelationshipToClient[item.RelationshipToVictimID]?.Description ?? "Unassigned", Counts = GetBlankDictionary(Headers) };
				foreach (var header in Headers)
					if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
						foreach (var subheader in header.SubHeaders)
							newRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
				Rows.Add(newRow);
			} else {
				foreach (var row in Rows)
					if (row.Code == item.RelationshipToVictimID)
						foreach (var header in Headers)
							if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
								foreach (var subheader in header.SubHeaders)
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
			}
		}
	}
}