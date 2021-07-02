using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Services.Hotline;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class HotlineSubReport : SubReportCountBuilder<PhoneHotline, HotlineLineItem> {
		public HotlineSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		/*
		 the numbers will not match the legacy app (in old data)
		 because in the legacy app call type 6 (call back - has client Id) 
		 and the ones that do not have a call type (null) are calculated 
		 in the subtotal but they are not in the results
		 */
		protected override IEnumerable<HotlineLineItem> PerformSelect(IQueryable<PhoneHotline> query) {
			return query.Select(q => new HotlineLineItem {
				Id = q.PH_ID,
				Center = q.Center.CenterName,
				CallTypeId = q.CallTypeID,
				CallDate = q.Date,
				NumberOfContacts = q.NumberOfContacts
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Call Type", "Hotline Call Date", "Number of Contacts" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, HotlineLineItem record) {
			csv.WriteField(record.Id);
			csv.WriteField(record.Center);
			csv.WriteField(Lookups.HotlineCallType[record.CallTypeId]?.Description);
			csv.WriteField(record.CallDate, "M/d/yyyy");
			csv.WriteField(record.NumberOfContacts);
		}

		protected override void CreateReportTables() {
			var table = new HotlineReportTable("Hotline Calls", 1) {
				Headers = GetNumberOfCallsHeaders(),
				HideSubheaders = true
			};
			foreach (var item in Lookups.HotlineCallType[ReportContainer.Provider])
				table.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(table);
		}

		private List<ReportTableHeader> GetNumberOfCallsHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader {
					Title = "Number of Calls",
					Code = ReportTableHeaderEnum.HotlineNumberOfCalls,
					SubHeaders = GetSingleSubHeader(ReportTableSubHeaderEnum.Total)
				}
			};
		}
	}

	public class HotlineLineItem {
		public int? Id { get; set; }
		public string Center { get; set; }
		public int? CallTypeId { get; set; }
		public DateTime? CallDate { get; set; }
		public int? NumberOfContacts { get; set; }
	}
}