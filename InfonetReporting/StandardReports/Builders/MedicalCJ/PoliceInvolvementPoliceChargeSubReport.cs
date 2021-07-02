using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Offenders;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Medical.PoliceInvolvement;

namespace Infonet.Reporting.StandardReports.Builders.MedicalCJ {
	public class MedicalCJPoliceInvolvementPoliceChargeSubReport : SubReportCountBuilder<PoliceCharge, MedicalCJPoliceInvolvementPoliceChargeLineItem> {
		public MedicalCJPoliceInvolvementPoliceChargeSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
			IsEndOfGroup = true;
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Offender ID", "Client ID", "Case ID", "Client Status", "Suspect Arrested", "Suspect Charged", "Suspect Charge Type", "Police Charge Type" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, MedicalCJPoliceInvolvementPoliceChargeLineItem record) {
			csv.WriteField(record.Id);
			if (ReportContainer.Provider == Provider.CAC)
				csv.WriteField(record.OffenderCode);
			else
				csv.WriteField(record.OffenderId);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.ArrestMade[record.SuspectArrested]?.Description);
			csv.WriteField(record.SuspectCharged);
			csv.WriteField(Lookups.CrimeClass[record.SuspectChargeType]?.Description);
			csv.WriteField(Lookups.Statute[record.PoliceChargeType]?.Description);
		}

		protected override void CreateReportTables() {
			var newAndOngoingTotal = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Total, false);

			var suspectArrestedGroup = new SuspectArrestedReportTable("Suspect Arrested", 4);
			suspectArrestedGroup.HideSubheaders = true;
			suspectArrestedGroup.HideSubtotal = true;
			suspectArrestedGroup.Headers = newAndOngoingTotal;
			suspectArrestedGroup.Rows.Add(new ReportRow { Code = 1, Title = "Yes" });
			ReportTableList.Add(suspectArrestedGroup);

			if (ReportContainer.Provider != Provider.CAC) {
				var suspectChargedGroup = new SuspectChargedReportTable("Suspect Charged", 5);
				suspectChargedGroup.HideSubheaders = true;
				suspectChargedGroup.HideSubtotal = true;
				suspectChargedGroup.Headers = newAndOngoingTotal;
				suspectChargedGroup.Rows.Add(new ReportRow { Code = 1, Title = "Yes" });
				ReportTableList.Add(suspectChargedGroup);
			}

			var chargeTypeGroup = new ChargeTypeReportTable("Charge Type", 6);
			chargeTypeGroup.HideSubheaders = true;
			chargeTypeGroup.Headers = newAndOngoingTotal;
			foreach (var item in Lookups.CrimeClass[ReportContainer.Provider])
				chargeTypeGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(chargeTypeGroup);

			var policeChargesGroup = new PoliceChargesReportTable("Police Charges", 7);
			policeChargesGroup.HideSubheaders = true;
			policeChargesGroup.Headers = newAndOngoingTotal;
			ReportTableList.Add(policeChargesGroup);
		}

		protected override IEnumerable<MedicalCJPoliceInvolvementPoliceChargeLineItem> PerformSelect(IQueryable<PoliceCharge> query) {
			if (ReportContainer.Provider == Provider.DV)
				query = query.Where(q => q.Offender.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.DVAdult);
			else if (ReportContainer.Provider == Provider.SA)
				query = query.Where(q => q.Offender.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.SAVictim);

			return query.Select(q => new MedicalCJPoliceInvolvementPoliceChargeLineItem {
				Id = q.PoliceChargeId,
				OffenderCode = q.Offender.OffenderListing.OffenderCode, //KMS DO perf?
				ClientCode = q.Offender.ClientCase.Client.ClientCode,
				CaseId = q.Offender.CaseId,
				OffenderId = q.OffenderId,
				ClientStatus = q.Offender.ClientCase.Client.ClientCases.GroupBy(c => c.ClientId).Select(c => c.Min(c2 => c2.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.Offender.ClientCase.Client.ClientCases.GroupBy(c => q.Offender.ClientId).Select(c => c.Min(c2 => c2.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				SuspectArrested = q.ArrestMadeId,
				SuspectCharged = q.StatuteId.HasValue || q.ChargeDate.HasValue,
				SuspectChargeType = q.ChargeTypeId,
				PoliceChargeType = q.StatuteId,
				DateOfArrest = q.ArrestDate
			});
		}
	}

	public class MedicalCJPoliceInvolvementPoliceChargeLineItem {
		public int? Id { get; set; }
		public string OffenderCode { get; set; }
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? SuspectArrested { get; set; }
		public bool SuspectCharged { get; set; }
		public int? SuspectChargeType { get; set; }
		public int? PoliceChargeType { get; set; }
		public DateTime? DateOfArrest { get; set; }
		public int? OffenderId { get; set; }
	}
}