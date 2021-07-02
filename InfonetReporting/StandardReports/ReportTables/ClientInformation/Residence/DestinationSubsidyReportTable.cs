using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Residence {
	public class DestinationSubsidyReportTable : ReportTable<ClientInformationResidenceLineItem> {
		public DestinationSubsidyReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(ClientInformationResidenceLineItem item) {
			foreach (ReportRow row in Rows) {
				foreach (ClientDeparture departure in item.Departures) {
					if (row.Code == departure.DestinationSubsidyID) {
						foreach (ReportTableHeader newOrOngoing in Headers) {
							// Check New vs. Ongoing - allow Total
							if (item.ClientStatus == newOrOngoing.Code || newOrOngoing.Code == ReportTableHeaderEnum.Total) {
								foreach (ReportTableSubHeader clientType in newOrOngoing.SubHeaders) {
									// Check if Client Type matches
									if (item.ClientTypeID == (int)clientType.Code || clientType.Code == ReportTableSubHeaderEnum.Total) {
										row.Counts[newOrOngoing.Code.ToString()][clientType.Code.ToString()] += 1;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}