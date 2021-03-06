using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.Medical {
	public class MedicalExamLocationReportTable : ReportTable<InvestigationMedicalLineItem> {
		private readonly HashSet<string> _primeIds = new HashSet<string>();

		public MedicalExamLocationReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(InvestigationMedicalLineItem item) {
			if (!_primeIds.Contains(item.ClientID + "-" + item.SiteLocationId))
				foreach (var row in Rows)
					if (row.Code == item.SiteLocationId)
						foreach (var header in Headers)
							if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
								foreach (var subheader in header.SubHeaders) {
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
									_primeIds.Add(item.ClientID + "-" + item.SiteLocationId);
								}
		}
	}
}