using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Offenders;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Medical.ProsecutionInvolvement;

namespace Infonet.Reporting.StandardReports.Builders.MedicalCJ {
	public class MedicalCJProsecutionInvolvementTrialChargesSubReport : SubReportCountBuilder<TrialCharge, MedicalCJProsecutionInvolvementTrialChargeLineItem> {
		public MedicalCJProsecutionInvolvementTrialChargesSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
			IsEndOfGroup = true;
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Offender ID", "Client ID", "Case ID", "Client Status", "State's Attorney Charges", "Court Disposition", "Sentence Types", "Suspect Charged" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, MedicalCJProsecutionInvolvementTrialChargeLineItem record) {
			csv.WriteField(record.Id);
			if (ReportContainer.Provider == Provider.CAC)
				csv.WriteField(record.OffenderCode);
			else
				csv.WriteField(record.OffenderId);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.Statute[record.StatesAttorneyCharge]?.Description);
			csv.WriteField(Lookups.Disposition[record.CourtDisposition]?.Description);
			csv.WriteField(string.Join("|", record.SentencesTypes.Select(st => Lookups.Sentence[st]?.Description ?? string.Empty)));
			csv.WriteField(record.SuspectCharged);
		}

		protected override void CreateReportTables() {
			var newAndOngoingTotalOnly = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Total, false);

			var suspectChargedTable = new ProsecutionSuspectChargedReportTable("Suspect Charged", 2);
			suspectChargedTable.Headers = newAndOngoingTotalOnly;
			suspectChargedTable.HideSubheaders = true;
			suspectChargedTable.HideSubtotal = true;
			suspectChargedTable.Rows.Add(new ReportRow { Title = "Yes", Code = (int)ShortAnswerEnum.Yes });
			ReportTableList.Add(suspectChargedTable);

			var saChargesTable = new ProsecutionStateAttorneyChargesReportTable("State's Attorney Charges", 8);
			saChargesTable.Headers = newAndOngoingTotalOnly;
			saChargesTable.HideSubheaders = true;
			ReportTableList.Add(saChargesTable);

			var courtDispositionTable = new ProsecutionCourtDispositionReportTable("Court Disposition", 9);
			courtDispositionTable.Headers = newAndOngoingTotalOnly;
			courtDispositionTable.HideSubheaders = true;
			foreach (var item in Lookups.Disposition[ReportContainer.Provider])
				courtDispositionTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(courtDispositionTable);

			var sentenceTypeTable = new ProsecutionSentenceTypeReportTable("Sentence Type", 10);
			sentenceTypeTable.Headers = newAndOngoingTotalOnly;
			sentenceTypeTable.HideSubheaders = true;
			foreach (var item in Lookups.Sentence[ReportContainer.Provider])
				sentenceTypeTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(sentenceTypeTable);
		}

		protected override IEnumerable<MedicalCJProsecutionInvolvementTrialChargeLineItem> PerformSelect(IQueryable<TrialCharge> query) {
			if (ReportContainer.Provider == Provider.DV)
				query = query.Where(q => q.Offender.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.DVAdult);
			else if (ReportContainer.Provider == Provider.SA)
				query = query.Where(q => q.Offender.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.SAVictim);

			return query.Select(q => new MedicalCJProsecutionInvolvementTrialChargeLineItem {
				Id = q.TrialChargeId,
				OffenderCode = q.Offender.OffenderListing.OffenderCode, //KMS DO perf?
				ClientCode = q.Offender.ClientCase.Client.ClientCode,
				CaseId = q.Offender.CaseId,
				ClientStatus = q.Offender.ClientCase.Client.ClientCases.GroupBy(c => c.ClientId).Select(c => c.Min(c2 => c2.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.Offender.ClientCase.Client.ClientCases.GroupBy(c => q.Offender.ClientCase.ClientId).Select(c => c.Min(c2 => c2.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				StatesAttorneyCharge = q.StatuteId,
				CourtDisposition = q.DispositionId,
				SentencesTypes = q.Sentences.Select(s => s.SentenceId),
				OffenderId = q.OffenderId,
				SuspectCharged = q.StatuteId.HasValue || q.ChargeDate.HasValue
			});
		}
	}

	public class MedicalCJProsecutionInvolvementTrialChargeLineItem {
		public int? Id { get; set; }
		public string OffenderCode { get; set; }
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? CourtDisposition { get; set; }
		public IEnumerable<int?> SentencesTypes { get; set; }
		public int? StatesAttorneyCharge { get; set; }
		public int? OffenderId { get; set; }
		public bool SuspectCharged { get; set; }
	}
}