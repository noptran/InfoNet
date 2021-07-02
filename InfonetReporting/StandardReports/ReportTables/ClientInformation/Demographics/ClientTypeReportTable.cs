using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class ClientTypeReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public ClientTypeReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			foreach (var row in Rows) {
				bool hasThisType = false;
				if (!item.ClientShelterTypeIDs.Any() && row.Code == (int)ShelterServiceEnum.Walkin)
					hasThisType = true;
				else if (item.ClientShelterTypeIDs.Contains(row.Code.Value))
					hasThisType = true;
				if (hasThisType)
					foreach (var newOrOngoing in Headers) // Check New vs. Ongoing - allow Total
						if (item.ClientStatus == newOrOngoing.Code || newOrOngoing.Code == ReportTableHeaderEnum.Total)
							foreach (var clientType in newOrOngoing.SubHeaders) // Check if Client Type matches
								if (item.ClientTypeID == (int)clientType.Code || clientType.Code == ReportTableSubHeaderEnum.Total)
									row.Counts[newOrOngoing.Code.ToString()][clientType.Code.ToString()] += 1;
			}
		}
	}
}