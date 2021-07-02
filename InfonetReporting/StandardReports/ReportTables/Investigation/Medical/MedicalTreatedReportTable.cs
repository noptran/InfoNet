using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.Medical {
	public class MedicalTreatedReportTable : ReportTable<InvestigationMedicalLineItem> {
		private readonly HashSet<int?> _primeIds = new HashSet<int?>();

		public MedicalTreatedReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(InvestigationMedicalLineItem item) {
			if (!_primeIds.Contains(item.ClientID))
				foreach (var row in Rows)
					if (row.Code == item.MedicalTreatmentId)
						foreach (var header in Headers)
							if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
								foreach (var subheader in header.SubHeaders) {
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
									_primeIds.Add(item.ClientID);
								}
		}
	}
}