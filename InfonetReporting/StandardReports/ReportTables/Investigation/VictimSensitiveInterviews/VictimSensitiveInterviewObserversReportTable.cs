using System.Collections.Generic;
using Infonet.Core.Collections;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.VictimSensitiveInterviews {
	public class VictimSensitiveInterviewObserversReportTable : ReportTable<VictimSensitiveInterviewLineItem> {
		private readonly HashSet<string> _primeIds = new HashSet<string>();

		public VictimSensitiveInterviewObserversReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(VictimSensitiveInterviewLineItem item) {
			foreach (var observer in item.ObserversList) {
				string uniqueKey = observer.ObserverID + "," + observer.VSI_ID;
				if (!_primeIds.Contains(uniqueKey))
					foreach (var row in Rows)
						if (row.Code == observer.ObserverID)
							foreach (var header in Headers)
								if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
									foreach (var subheader in header.SubHeaders)
										if (subheader.Code.ToInt32() == item.ClientTypeID || subheader.Code == ReportTableSubHeaderEnum.Total) {
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
											_primeIds.Add(uniqueKey);
										}
			}
		}
	}
}