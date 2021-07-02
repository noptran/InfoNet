using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Services.DirectServices;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class DirectClientReferralsSubReport : SubReportCountBuilder<ClientReferralDetail, ReferralLineItem> {
		public DirectClientReferralsSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override IEnumerable<ReferralLineItem> PerformSelect(IQueryable<ClientReferralDetail> query) {
			return query.Select(q => new ReferralLineItem {
				Id = q.ReferralDetailID,
				Center = q.Center.CenterName,
				ClientID = q.ClientID,
				CaseID = q.CaseID,
				ClientCode = q.ClientCase.Client.ClientCode,
				ClientTypeId = q.ClientCase.Client.ClientTypeId,
				ReferralTypeID = q.ReferralTypeID,
				ReferralDate = q.ReferralDate
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Client ID", "Case ID", "Client Type", "Referral Type", "Referral Date" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ReferralLineItem record) {
			csv.WriteField(record.Id);
			csv.WriteField(record.Center);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(Lookups.ClientType[record.ClientTypeId]?.Description);
			csv.WriteField(Lookups.ReferralType[record.ReferralTypeID]?.Description);
			csv.WriteField(record.ReferralDate, "M/d/yyyy");
		}

		protected override void CreateReportTables() {
			var referralServices = new ReferralsReportTable("Client Referrals", 2) {
				Headers = GetNumberOfClientsNumberOfContactsHoursOfServiceHeaders(),
				UseNonDuplicatedSubtotal = true
			};
			foreach (var each in Lookups.ReferralType[ReportContainer.Provider])
				referralServices.Rows.Add(GetReportRowFromLookup(each));
			ReportTableList.Add(referralServices);
		}

		private List<ReportTableHeader> GetNumberOfClientsNumberOfContactsHoursOfServiceHeaders() {
			var subHeaders = GetClientTypeSubHeaders(ReportContainer.Provider);
			return new List<ReportTableHeader> {
				new ReportTableHeader {
					Title = "Number of Clients Receiving Service",
					Code = ReportTableHeaderEnum.NumberOfClientsReceivingServices,
					SubHeaders = subHeaders
				},
				new ReportTableHeader {
					Title = "Number of Contacts",
					Code = ReportTableHeaderEnum.NumberOfContacts,
					SubHeaders = subHeaders
				},
				new ReportTableHeader {
					Title = "Hours of Service",
					Code = ReportTableHeaderEnum.HoursOfService,
					SubHeaders = subHeaders
				}
			};
		}
	}

	public class ReferralLineItem {
		public int? Id { get; set; }
		public string Center { get; set; }
		public int? ClientID { get; set; }
		public int? CaseID { get; set; }
		public string ClientCode { get; set; }
		public int? ClientTypeId { get; set; }
		public int? ReferralTypeID { get; set; }
		public DateTime? ReferralDate { get; set; }
	}
}