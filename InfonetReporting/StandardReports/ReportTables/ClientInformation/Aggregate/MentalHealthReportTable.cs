using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Aggregate {
	public class MentalHealthReportTable : ReportTable<ClientInformationAggregateLineItem> {
		public MentalHealthReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(ClientInformationAggregateLineItem item) {
			foreach (ReportRow row in Rows) {
				if (row.Code == item.CenterId && item.TypeId == (int)HivMentalSubstanceEnum.MentalHealthProblem) {
					foreach (ReportTableHeader counts in Headers) {
						foreach (ReportTableSubHeader total in counts.SubHeaders) {
                            row.Counts[counts.Code.ToString()][total.Code.ToString()] += counts.Code == ReportTableHeaderEnum.HIVAdultCount ? item.AdultsNo.Value : item.ChildrenNo.Value;
                        }
                    }
				}
			}
		}
	}
}