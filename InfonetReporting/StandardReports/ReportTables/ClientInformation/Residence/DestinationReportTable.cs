using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Residence {
	public class DestinationReportTable : ReportTable<ClientInformationResidenceLineItem> {
		public DestinationReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(ClientInformationResidenceLineItem item) {

            CheckForDepartures(item);
			foreach (ReportRow row in Rows) {
				foreach (ClientDeparture departure in item.Departures) {
					if (row.Code == departure.DestinationID) {
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

        private void CheckForDepartures(ClientInformationResidenceLineItem item) {
            if (!item.Departures.Any())
                return;
                    
            foreach (ClientDeparture current in item.Departures.ToList()) {
                bool removeDeparture = true;
                foreach (ServiceDetailOfClient service in item.Services.ToList()) {
                    if (current.DepartureDate >= service.ShelterBegDate && current.DepartureDate <= service.ShelterEndDate) {
                        removeDeparture = false;
                        break;
                    }
                }

                if (removeDeparture) {
                    ((List<ClientDeparture>)item.Departures).Remove(current);
                }
            }

        }
    }
}