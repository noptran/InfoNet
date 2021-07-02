using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Medical.MedicalResponse;

namespace Infonet.Reporting.StandardReports.Builders.MedicalCJ {
	public class MedicalSystemInvolvementConflictScaleSubReport : SubReportCountBuilder<ClientCase, MedicalSystemInvolvementClientConflictLineItem> {
		public MedicalSystemInvolvementConflictScaleSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
			IsEndOfGroup = true;
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Threw something at your victim", "Pushed, grabbed or shoved your victim", "Slapped your victim", "Kicked, bit or hit your victim with a fist", "Hit or tried to hit your victim with something", "Beat up your victim", "Choked your victim", "Threatened your victim with a knife or gun", "Used a knife or fired a gun" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, MedicalSystemInvolvementClientConflictLineItem record) {
			csv.WriteField(record.ClientId);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(record.ClientConflictScale?.Threw);
			csv.WriteField(record.ClientConflictScale?.Pushed);
			csv.WriteField(record.ClientConflictScale?.Slapped);
			csv.WriteField(record.ClientConflictScale?.Kicked);
			csv.WriteField(record.ClientConflictScale?.Hit);
			csv.WriteField(record.ClientConflictScale?.BeatUp);
			csv.WriteField(record.ClientConflictScale?.Choked);
			csv.WriteField(record.ClientConflictScale?.Threatened);
			csv.WriteField(record.ClientConflictScale?.Used);
		}

		protected override void CreateReportTables() {
			var newAndOngoingTotalOnly = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Total, false);

			var severityGroup = new MedicalCJResponseSeverityOfAbuseReportTable("Severity of Abuse", 7);
			severityGroup.HideSubheaders = true;
            severityGroup.UseNonDuplicatedSubtotal = true;
			severityGroup.Headers = newAndOngoingTotalOnly;
			foreach (ClientConflictScaleEnum item in Enum.GetValues(typeof(ClientConflictScaleEnum)))
				severityGroup.Rows.Add(new ReportRow { Title = item.GetDisplayName(), Code = item.ToInt32() });
			ReportTableList.Add(severityGroup);
		}

		protected override IEnumerable<MedicalSystemInvolvementClientConflictLineItem> PerformSelect(IQueryable<ClientCase> query) {
			if (ReportContainer.Provider == Provider.DV)
				query = query.Where(q => q.Client.ClientTypeId == (int)ClientTypeEnum.DVAdult);

			return query.Select(q => new MedicalSystemInvolvementClientConflictLineItem {
				ClientId = q.ClientId,
				ClientCode = q.Client.ClientCode,
				CaseId = q.CaseId,
				ClientStatus = q.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				ClientConflictScale = q.ClientConflictScale
			});
		}
	}

	public class MedicalSystemInvolvementClientConflictLineItem {
		public int? ClientId { get; set; }
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public ClientConflictScale ClientConflictScale { get; set; }
	}
}