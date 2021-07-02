using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Medical.MedicalResponse;

namespace Infonet.Reporting.StandardReports.Builders.MedicalCJ {
	public class MedicalSystemInvolvementSubReport : SubReportCountBuilder<ClientCJProcess, MedicalSystemInvolvementLineItem> {
		public MedicalSystemInvolvementSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Medical Facility Visit", "Treatment for Injuries", "Seriousness of Injuries", "Photos Taken", "Type of Medical Facility", "Evidence Kit Used" ,"Treated by SANE"}; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, MedicalSystemInvolvementLineItem record) {
			csv.WriteField(record.ClientId);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.YesNo[record.MedicalVisitId]?.Description);
			csv.WriteField(Lookups.YesNo[record.MedicalTreatmentId]?.Description);
			csv.WriteField(Lookups.InjurySeverity[record.InjuryId]?.Description);
			csv.WriteField(Lookups.YesNo[record.PhotosTakenId]?.Description);
			csv.WriteField(Lookups.MedicalTreatmentLocation[record.MedWhereId]?.Description);
			csv.WriteField(Lookups.YesNo[record.EvidKitId]?.Description);
            csv.WriteField(Lookups.YesNo[record.SANETreatedId]?.Description);
        }

        protected override void CreateReportTables() {
            var newAndOngoingTotalOnly = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Total, false);

            var medicalVisitGroup = new MedicalCJResponseFacilityVisitReportTable("Medical Facility Visit", 1);
           // medicalVisitGroup.HideSubtotal = true;
            medicalVisitGroup.HideSubheaders = true;
            medicalVisitGroup.Headers = newAndOngoingTotalOnly;
            foreach (var item in Lookups.YesNo[ReportContainer.Provider])
                medicalVisitGroup.Rows.Add(GetReportRowFromLookup(item));
            medicalVisitGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });

            ReportTableList.Add(medicalVisitGroup);

            var treatedGroup = new MedicalCJResponseTreatedReportTable("Treated For Injuries", 2);
            treatedGroup.HideSubheaders = true;         
            treatedGroup.Headers = newAndOngoingTotalOnly;
            foreach (var item in Lookups.YesNo[ReportContainer.Provider])
                treatedGroup.Rows.Add(GetReportRowFromLookup(item));
            treatedGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });

            ReportTableList.Add(treatedGroup);

            var seriousnessGroup = new MedicalCJResponseSeriousnessReportTable("Seriousness of Injuries", 3);
            seriousnessGroup.HideSubheaders = true;
            seriousnessGroup.Headers = newAndOngoingTotalOnly;
            foreach (var item in Lookups.InjurySeverity[ReportContainer.Provider])
                seriousnessGroup.Rows.Add(GetReportRowFromLookup(item));
            seriousnessGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });

            ReportTableList.Add(seriousnessGroup);

            var photosGroup = new MedicalCJResponsePhotosReportTable("Photos Taken", 4);
            photosGroup.HideSubheaders = true;
            photosGroup.Headers = newAndOngoingTotalOnly;
            foreach (var item in Lookups.YesNo[ReportContainer.Provider])
                photosGroup.Rows.Add(GetReportRowFromLookup(item));
            photosGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });

            ReportTableList.Add(photosGroup);

            var facilityGroup = new MedicalCJResponseFacilityTypeReportTable("Type of Medical Facility", 5);
            facilityGroup.HideSubheaders = true;
            facilityGroup.Headers = newAndOngoingTotalOnly;
            foreach (var item in Lookups.MedicalTreatmentLocation[ReportContainer.Provider])
                facilityGroup.Rows.Add(GetReportRowFromLookup(item));
            facilityGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });

            ReportTableList.Add(facilityGroup);

            var evidenceKitGroup = new MedicalCJResponseEvidenceKitReportTable("Evidence Kit Used", 6);
            evidenceKitGroup.HideSubheaders = true;
            evidenceKitGroup.Headers = newAndOngoingTotalOnly;
            foreach (var item in Lookups.YesNo[ReportContainer.Provider])
                evidenceKitGroup.Rows.Add(GetReportRowFromLookup(item));
            evidenceKitGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });

            ReportTableList.Add(evidenceKitGroup);

            // SA ONLY
            if (ReportContainer.Provider == Provider.SA) {
                var treatedBySANEGroup = new MedicalCJResponseTreatedBySANEReportTable("Treated by SANE", 7);
                treatedBySANEGroup.HideSubheaders = true;    
                treatedBySANEGroup.Headers = newAndOngoingTotalOnly;
                foreach (var item in Lookups.YesNo[ReportContainer.Provider])
                    treatedBySANEGroup.Rows.Add(GetReportRowFromLookup(item));
                treatedBySANEGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });

                ReportTableList.Add(treatedBySANEGroup);
            }
        }

		protected override IEnumerable<MedicalSystemInvolvementLineItem> PerformSelect(IQueryable<ClientCJProcess> query) {
			switch (ReportContainer.Provider) {
				case Provider.DV:
					query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.DVAdult);
					break;
				case Provider.SA:
					query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.SAVictim);
					break;
			}

            return query.Select(q => new MedicalSystemInvolvementLineItem {
                ClientId = q.ClientId,
                ClientCode = q.ClientCase.Client.ClientCode,
                CaseId = q.CaseId,
                ClientStatus = q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
                MedicalVisitId = q.MedicalVisitId,
                MedicalTreatmentId = q.MedicalTreatmentId,
                InjuryId = q.InjuryId,
                EvidKitId = q.EvidKitId,
                PhotosTakenId = q.PhotosTakenId,
                SANETreatedId = q.SANETreatedId,
				MedWhereId = q.MedWhereId
			});
		}
	}

	public class MedicalSystemInvolvementLineItem {
		public int? ClientId { get; set; }
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? MedicalVisitId { get; set; }
		public int? MedicalTreatmentId { get; set; }
		public int? InjuryId { get; set; }
		public int? EvidKitId { get; set; }
		public int? PhotosTakenId { get; set; }
        public int? SANETreatedId { get; set; }
        public int? MedWhereId { get; set; }
	}
}