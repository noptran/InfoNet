using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Aggregate {
	public class HivAidsReportTable : ReportTable<ClientInformationAggregateLineItem> {
		public HivAidsReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationAggregateLineItem item) {
			foreach (var row in Rows)
				if (row.Code == item.CenterId && item.TypeId == (int)HivMentalSubstanceEnum.HIVAIDS)
					foreach (var counts in Headers) {
						foreach (var total in counts.SubHeaders)
							row.Counts[counts.Code.ToString()][total.Code.ToString()] += counts.Code == ReportTableHeaderEnum.HIVAdultCount ? item.AdultsNo.Value : item.ChildrenNo.Value;
					}
		}
	}
}