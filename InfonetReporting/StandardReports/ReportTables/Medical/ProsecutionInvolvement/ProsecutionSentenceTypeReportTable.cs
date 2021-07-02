using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.ProsecutionInvolvement {
	public class ProsecutionSentenceTypeReportTable : ReportTable<MedicalCJProsecutionInvolvementTrialChargeLineItem> {
		public ProsecutionSentenceTypeReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}

		public override void CheckAndApply(MedicalCJProsecutionInvolvementTrialChargeLineItem item) {
			if (item.SentencesTypes.Count() != 0) {
				foreach (var sentenceType in item.SentencesTypes) {
					foreach (ReportRow row in Rows) {
						if (sentenceType == row.Code) {
							foreach (ReportTableHeader header in Headers) {
								if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total) {
									row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += 1;
								}
							}
						}
					}
				}
			}
		}
	}
}