using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Residence {
	public class ResidenceTypeReportTable : ReportTable<ClientInformationResidenceLineItem> {
		public ResidenceTypeReportTable(string title, int displayOrder) : base(title, displayOrder) { }        
		public override void CheckAndApply(ClientInformationResidenceLineItem item) {
            CheckForResidences(item);
			foreach (ReportRow row in Rows) {
				foreach (TwnTshipCounty twn in item.Residences) {
					if (row.Code == twn.ResidenceTypeID) {
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
        private void CheckForResidences(ClientInformationResidenceLineItem item) {

            if (!item.Residences.Any()) 
                item.Residences = new List<TwnTshipCounty>();

            string prevlocid = string.Empty;
            int serviceCtr = item.Services.Count();

            foreach (var current in item.Services.OrderBy(x => x.CityTownTownshpID).ThenBy(x => x.ServiceDetailID).ToList()) {
                if (current.CityTownTownshpID == null)
                    continue;

                if (current.CityTownTownshpID.Value.ToString() == prevlocid) {
                    serviceCtr -= 1;
                }
                prevlocid = current.CityTownTownshpID.Value.ToString();
            }

            for (int i = item.Residences.Count(); i < serviceCtr; i++) {
                ((List<TwnTshipCounty>)item.Residences).Add(new TwnTshipCounty {
                    ResidenceTypeID = null
                });
            }
        }
    }
}