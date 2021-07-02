using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Investigation.Medical;

namespace Infonet.Reporting.StandardReports.Builders.Investigation {
	public class MedicalSubReportBuilder : SubReportCountBuilder<ClientCJProcess, InvestigationMedicalLineItem> {
		public MedicalSubReportBuilder(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Client Type", "Medical Visit", "Medical Treatment", "Medical Facility Type", "Exam Location", "Finding", "Exam Type", "Exam Completed", "Evidence Kit Used", "Colposcope Used", "Exam Before or After VSI" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, InvestigationMedicalLineItem record) {
			csv.WriteField(record.ClientID);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.ClientType[record.ClientTypeId]?.Description);
			csv.WriteField(Lookups.YesNo[record.MedicalVisitId]?.Description);
			csv.WriteField(Lookups.YesNo[record.MedicalTreatmentId]?.Description);
			csv.WriteField(Lookups.MedicalTreatmentLocation[record.MedWhereId]?.Description);
			csv.WriteField(Lookups.SiteLocation[record.SiteLocationId]?.Description);
			csv.WriteField(Lookups.MedicalExamFinding[record.FindingId]?.Description);
			csv.WriteField(Lookups.MedicalExamType[record.ExamTypeId]?.Description);
			csv.WriteField(Lookups.YesNo[record.ExamCompletedId]?.Description);
			csv.WriteField(Lookups.YesNo[record.EvidKitId]?.Description);
			csv.WriteField(Lookups.YesNo[record.ColposcopeUsedId]?.Description);
			csv.WriteField(Lookups.BeforeAfter[record.BeforeAfterId]?.Description);
		}

		protected override void CreateReportTables() {
			var newAndOngoingVictim = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false);

			var totalVictimsGroup = new MedicalTotalVictimsReportTable("Total Number of Victims", 1);
			totalVictimsGroup.HideTitle = true;
			totalVictimsGroup.HideSubtotal = true;
			totalVictimsGroup.HideSubheaders = true;
			totalVictimsGroup.Headers = newAndOngoingVictim;
			totalVictimsGroup.Rows.Add(new ReportRow { Title = "Total Number of Victims" });
			ReportTableList.Add(totalVictimsGroup);

			var totalVictimCasesGroup = new MedicalTotalVictimCasesReportTable("Total Number of Victim Cases", 2);
			totalVictimCasesGroup.HideTitle = true;
			totalVictimCasesGroup.HideSubtotal = true;
			totalVictimCasesGroup.HideSubheaders = true;
			totalVictimCasesGroup.Headers = newAndOngoingVictim;
			totalVictimCasesGroup.Rows.Add(new ReportRow { Title = "Total Number of Victim Cases" });
			ReportTableList.Add(totalVictimCasesGroup);

			var medicalVisitGroup = new MedicalFacilityVisitReportTable("Medical Facility Visit", 3);
			medicalVisitGroup.HideSubtotal = true;
			medicalVisitGroup.HideSubheaders = true;
			medicalVisitGroup.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.YesNo[ReportContainer.Provider])
				medicalVisitGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(medicalVisitGroup);

			var facilityGroup = new MedicalFacilityTypeReportTable("Type of Medical Facility", 4);
			facilityGroup.HideSubheaders = true;
			facilityGroup.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.MedicalTreatmentLocation[ReportContainer.Provider])
				facilityGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(facilityGroup);

			var treatedGroup = new MedicalTreatedReportTable("Treated For Injuries", 5);
			treatedGroup.HideSubheaders = true;
			treatedGroup.HideSubtotal = true;
			treatedGroup.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.YesNo[ReportContainer.Provider])
				treatedGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(treatedGroup);

			var examCompletedTable = new MedicalExamCompletedReportTable("Exam Completed", 6);
			examCompletedTable.HideSubheaders = true;
			examCompletedTable.HideSubtotal = true;
			examCompletedTable.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.YesNo[ReportContainer.Provider])
				examCompletedTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(examCompletedTable);

			var examBeforeTable = new MedicalExamCompletedBeforeOrAfterReportTable("Exam Completed Before or After VSI", 7);
			examBeforeTable.HideSubheaders = true;
			examBeforeTable.HideSubtotal = true;
			examBeforeTable.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.BeforeAfter[ReportContainer.Provider])
				examBeforeTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(examBeforeTable);

			var examTypeTable = new MedicalExamTypeReportTable("Type of Exam", 8);
			examTypeTable.HideSubheaders = true;
			examTypeTable.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.MedicalExamType[ReportContainer.Provider])
				examTypeTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(examTypeTable);

			var examLocationTable = new MedicalExamLocationReportTable("Exam Location", 9);
			examLocationTable.HideSubheaders = true;
			examLocationTable.HideSubtotal = true;
			examLocationTable.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.SiteLocation[ReportContainer.Provider])
				examLocationTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(examLocationTable);

			var evidenceKitGroup = new MedicalEvidenceKitReportTable("Evidence Kit Used", 10);
			evidenceKitGroup.HideSubheaders = true;
			evidenceKitGroup.HideSubtotal = true;
			evidenceKitGroup.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.YesNo[ReportContainer.Provider])
				evidenceKitGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(evidenceKitGroup);

			var colposcopeTable = new MedicalColposcopeReportTable("Colposcope or Medscope Used", 11);
			colposcopeTable.HideSubheaders = true;
			colposcopeTable.HideSubtotal = true;
			colposcopeTable.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.YesNo[ReportContainer.Provider])
				colposcopeTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(colposcopeTable);

			var findingsTable = new MedicalFindingsReportTable("Findings", 12);
			findingsTable.HideSubheaders = true;
			findingsTable.Headers = newAndOngoingVictim;
			foreach (var item in Lookups.MedicalExamFinding[ReportContainer.Provider])
				findingsTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(findingsTable);
		}

		protected override IEnumerable<InvestigationMedicalLineItem> PerformSelect(IQueryable<ClientCJProcess> query) {
			switch (ReportContainer.Provider) {
				case Provider.DV:
					query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.DVAdult);
					break;
				case Provider.SA:
					query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.SAVictim);
					break;
			}

			return query.Select(q => new InvestigationMedicalLineItem {
				ClientID = q.ClientId,
				ClientCode = q.ClientCase.Client.ClientCode,
				CaseID = q.CaseId,
				ClientTypeId = q.ClientCase.Client.ClientTypeId,
				ClientStatus = q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				MedicalVisitId = q.MedicalVisitId,
				MedicalTreatmentId = q.MedicalTreatmentId,
				EvidKitId = q.EvidKitId,
				MedWhereId = q.MedWhereId,
				ColposcopeUsedId = q.ColposcopeUsedId,
				BeforeAfterId = q.BeforeAfterId,
				SiteLocationId = q.SiteLocationId,
				ExamCompletedId = q.ExamCompletedId,
				FindingId = q.FindingId,
				ExamTypeId = q.ExamTypeId
			});
		}
	}

	public class InvestigationMedicalLineItem {
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? CaseID { get; set; }
		public int? ClientTypeId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? MedicalVisitId { get; set; }
		public int? MedicalTreatmentId { get; set; }
		public int? EvidKitId { get; set; }
		public int? MedWhereId { get; set; }
		public int? ColposcopeUsedId { get; set; }
		public int? BeforeAfterId { get; set; }
		public int? SiteLocationId { get; set; }
		public int? ExamCompletedId { get; set; }
		public int? FindingId { get; set; }
		public int? ExamTypeId { get; set; }
	}
}