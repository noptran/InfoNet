using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Residence;

namespace Infonet.Reporting.StandardReports.Builders.ClientInformation {
    public class ClientInformationResidenceDestinationSubReport : SubReportCountBuilder<ClientCase, ClientInformationResidenceLineItem> {
        public ClientInformationResidenceDestinationSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

        protected override string[] CsvHeaders {
            get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Client Type", "Residence Type", "Length Of Stay", "Destination Type", "Destination Tenure", "Destination Subsidy", "Reason For Leaving", "Previous Shelter Used", "Previous Shelter Date", "Previous Service Used", "Previous Service Date", "Most Recent Shelter Begin Date" }; }
        }

        protected override void WriteCsvRecord(CsvWriter csv, ClientInformationResidenceLineItem record) {
            CheckForResidences(record);
            csv.WriteField(record.ClientID);
            csv.WriteField(record.ClientCode);
            csv.WriteField(record.CaseID);
            csv.WriteField(record.ClientStatus);
            csv.WriteField(Lookups.ClientType[record.ClientTypeID]?.Description);
            csv.WriteField(string.Join("|", record.Residences.Select(r => Lookups.ResidenceType[r.ResidenceTypeID]?.Description ?? "Unassigned").DefaultIfEmpty("Unassigned")));
            csv.WriteField(string.Join("|", record.Residences.Select(r => Lookups.LengthOfStay[r.LengthOfStayInResidenceID]?.Description ?? "Unassigned").DefaultIfEmpty("Unassigned")));
            csv.WriteField(string.Join("|", record.Departures.Select(d => Lookups.ResidenceType[d.DestinationID]?.Description ?? "Unassigned").DefaultIfEmpty("Unassigned")));
            csv.WriteField(string.Join("|", record.Departures.Select(d => Lookups.DestinationTenure[d.DestinationTenureID]?.Description ?? "Unassigned").DefaultIfEmpty("Unassigned")));
            csv.WriteField(string.Join("|", record.Departures.Select(d => Lookups.DestinationSubsidy[d.DestinationSubsidyID]?.Description ?? "Unassigned").DefaultIfEmpty("Unassigned")));
            csv.WriteField(string.Join("|", record.Departures.Select(d => Lookups.ReasonForLeaving[d.ReasonForLeavingID]?.Description ?? "Unassigned").DefaultIfEmpty("Unassigned")));
            if (record.HasPreviousServiceUse) {
                csv.WriteField(record.PreviousServiceUse.PrevShelterUseId.HasValue ? ((ShortAnswerEnum)record.PreviousServiceUse.PrevShelterUseId).ToString() : string.Empty);
                csv.WriteField(record.PreviousServiceUse.PrevShelterDate, "M/d/yyyy");
                csv.WriteField(record.PreviousServiceUse.PrevServiceUseId.HasValue ? ((ShortAnswerEnum)record.PreviousServiceUse.PrevServiceUseId).ToString() : string.Empty);
                csv.WriteField(record.PreviousServiceUse.PrevServiceDate, "M/d/yyyy");
                csv.WriteField(record.MostRecentShelterBeginDate, "M/d/yyyy");
            } else {
                csv.WriteEmptyFields(5);
            }
        }

        private void CheckForResidences(ClientInformationResidenceLineItem item) {

            if (!item.Residences.Any())
                item.Residences = new List<TwnTshipCounty>();

            string prevlocid = string.Empty;
            foreach (var current in item.Services.OrderBy(x => x.CityTownTownshpID).ToList()) {

                if (current.CityTownTownshpID == null)
                    continue;

                if (current.CityTownTownshpID.Value.ToString() == prevlocid) {
                    ((List<ServiceDetailOfClient>)item.Services).Remove(current);
                }
                prevlocid = current.CityTownTownshpID.Value.ToString();
            }

            for (int i = item.Residences.Count(); i < item.Services.Count(); i++) {
                ((List<TwnTshipCounty>)item.Residences).Add(new TwnTshipCounty {
                    ResidenceTypeID = null
                });
            }
        }

        protected override IEnumerable<ClientInformationResidenceLineItem> PerformSelect(IQueryable<ClientCase> query) {
            int[] shelterIds = { (int)ShelterServiceEnum.OnsiteShelter, (int)ShelterServiceEnum.OffsiteShelter, (int)ShelterServiceEnum.TransitionalHousing };

            query = query.Where(m => m.ServiceDetailsOfClient.Any(sd => shelterIds.Contains(sd.ServiceID))
            && m.ServiceDetailsOfClient.Any(
                sd => shelterIds.Contains(sd.ServiceID)
                && sd.ShelterBegDate.HasValue
                && (sd.ShelterBegDate <= ReportContainer.EndDate
                    && (sd.ShelterEndDate >= ReportContainer.StartDate
                    || !sd.ShelterEndDate.HasValue)
                    || m.PreviousServiceUse != null
                    )
                ));
            var results = query.Select(t => new ClientInformationResidenceLineItem {
                ClientID = t.ClientId,
                ClientCode = t.Client.ClientCode,
                CaseID = t.CaseId,
                ClientStatus = t.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && t.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
                ClientTypeID = t.Client.ClientTypeId,
                Residences = t.Client.TwnTshipCounty.Where(x => x.ServiceDetailsOfClient.Any(sd => sd.ShelterBegDate.HasValue
                 && (sd.ShelterBegDate <= ReportContainer.EndDate
                     && (sd.ShelterEndDate >= ReportContainer.StartDate
                     || !sd.ShelterEndDate.HasValue)))).ToList(),
                Services = t.ServiceDetailsOfClient.Where(sd => sd.ShelterBegDate.HasValue
                 && (sd.ShelterBegDate <= ReportContainer.EndDate
                     && (sd.ShelterEndDate >= ReportContainer.StartDate
                     || !sd.ShelterEndDate.HasValue))),
                Departures = t.ClientDepartures.Where(d => d.DepartureDate >= ReportContainer.StartDate && d.DepartureDate <= ReportContainer.EndDate),
                HasPreviousServiceUse = t.ServiceDetailsOfClient.Any(s => s.ShelterBegDate <= ReportContainer.EndDate && (s.ShelterEndDate <= ReportContainer.EndDate || !s.ShelterEndDate.HasValue))
                    && (t.PreviousServiceUse.PrevServiceUseId == (int)ShortAnswerEnum.Yes || t.PreviousServiceUse.PrevShelterUseId == (int)ShortAnswerEnum.Yes),
                PreviousServiceUse = t.PreviousServiceUse,
                MostRecentShelterBeginDate = t.ServiceDetailsOfClient.Select(sd => sd.ShelterBegDate).Min(),
                MostRecentShelterEndDate = t.ServiceDetailsOfClient.Select(sd => sd.ShelterEndDate).Min()
            });

            return results;
        }

        protected override void CreateReportTables() {
            var residenceTypeGroup = new ResidenceTypeReportTable("Residence Type", 1);
            residenceTypeGroup.Headers = GetNewAndOngoingHeaders();
            foreach (var item in Lookups.ResidenceType[ReportContainer.Provider])
                residenceTypeGroup.Rows.Add(GetReportRowFromLookup(item));
            residenceTypeGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
            ReportTableList.Add(residenceTypeGroup);

            var lengthOfStayGroup = new LengthOfStayReportTable("Length Of Stay in Previous Place", 2);
            lengthOfStayGroup.Headers = GetNewAndOngoingHeaders();
            foreach (var item in Lookups.LengthOfStay[ReportContainer.Provider])
                lengthOfStayGroup.Rows.Add(GetReportRowFromLookup(item));
            lengthOfStayGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
            ReportTableList.Add(lengthOfStayGroup);

            var destinationGroup = new DestinationReportTable("Destination", 3);
            destinationGroup.Headers = GetNewAndOngoingHeaders();
            foreach (var item in Lookups.Destination[ReportContainer.Provider])
                destinationGroup.Rows.Add(GetReportRowFromLookup(item));
            destinationGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
            ReportTableList.Add(destinationGroup);

            var destinationTenureGroup = new DestinationTenureReportTable("Destination Tenure", 4);
            destinationTenureGroup.Headers = GetNewAndOngoingHeaders();
            foreach (var item in Lookups.DestinationTenure[ReportContainer.Provider])
                destinationTenureGroup.Rows.Add(GetReportRowFromLookup(item));
            destinationTenureGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
            ReportTableList.Add(destinationTenureGroup);

            var destinationSubsidyGroup = new DestinationSubsidyReportTable("Destination Subsidy", 5);
            destinationSubsidyGroup.Headers = GetNewAndOngoingHeaders();
            foreach (var item in Lookups.DestinationSubsidy[ReportContainer.Provider])
                destinationSubsidyGroup.Rows.Add(GetReportRowFromLookup(item));
            destinationSubsidyGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
            ReportTableList.Add(destinationSubsidyGroup);

            var leaveReasonGroup = new LeaveReasonReportTable("Reason for Leaving", 5);
            leaveReasonGroup.Headers = GetNewAndOngoingHeaders();
            foreach (var item in Lookups.ReasonForLeaving[ReportContainer.Provider])
                leaveReasonGroup.Rows.Add(GetReportRowFromLookup(item));
            leaveReasonGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
            ReportTableList.Add(leaveReasonGroup);

            var shelterHomelessTable = new ShelterHomelessReportTable("Shelter/Homeless Use", 6);
            shelterHomelessTable.Headers = GetHeadersForShelter();
            shelterHomelessTable.Rows.Add(new ReportRow { Title = "0-3 months ago", Order = 1, Code = (int)ShelterUseEnum.From0to3MonthsAgo });
            shelterHomelessTable.Rows.Add(new ReportRow { Title = "4-6 months ago", Order = 2, Code = (int)ShelterUseEnum.From4to6MonthsAgo });
            shelterHomelessTable.Rows.Add(new ReportRow { Title = "7-9 months ago", Order = 3, Code = (int)ShelterUseEnum.From7to9MonthsAgo });
            shelterHomelessTable.Rows.Add(new ReportRow { Title = "10-12 months ago", Order = 4, Code = (int)ShelterUseEnum.From10to12MonthsAgo });
            shelterHomelessTable.Rows.Add(new ReportRow { Title = "Unknown", Order = 5, Code = (int)ShelterUseEnum.Unknown });
            ReportTableList.Add(shelterHomelessTable);
        }

        private List<ReportTableHeader> GetHeadersForShelter() {
            return new List<ReportTableHeader> {
                new ReportTableHeader { Code = ReportTableHeaderEnum.DVShelterUse, Title = "DV Shelter Use", SubHeaders = GetClientTypeSubHeaders(ReportContainer.Provider) },
                new ReportTableHeader { Code = ReportTableHeaderEnum.HomelessServiceUse, Title = "Homeless Service Use", SubHeaders = GetClientTypeSubHeaders(ReportContainer.Provider) },
                new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = "Total(Unduplicated)", SubHeaders = GetClientTypeSubHeaders(ReportContainer.Provider) }
            };
        }
    }

    public class ClientInformationResidenceLineItem {
        public int? ClientID { get; set; }
        public string ClientCode { get; set; }
        public int? CaseID { get; set; }
        public ReportTableHeaderEnum ClientStatus { get; set; }
        public int? ClientTypeID { get; set; }
        public IEnumerable<TwnTshipCounty> Residences { get; set; }
        public IEnumerable<ClientDeparture> Departures { get; set; }
        public IEnumerable<ServiceDetailOfClient> Services { get; set; }

        public virtual PreviousServiceUse PreviousServiceUse { get; set; }
        public DateTime? MostRecentShelterBeginDate { get; set; }
        public DateTime? MostRecentShelterEndDate { get; set; }
        public bool HasPreviousServiceUse { get; set; }
    }
}