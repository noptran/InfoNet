using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Aggregate;

namespace Infonet.Reporting.StandardReports.Builders.ClientInformation {
	public class ClientInformationAggregateSubReport : SubReportCountBuilder<HivMentalSubstance, ClientInformationAggregateLineItem> {
		public ClientInformationAggregateSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Type", "Number of Adults", "Number of Children" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ClientInformationAggregateLineItem record) {
			csv.WriteField(record.Id);
			csv.WriteField(record.CenterName);
			csv.WriteField(record.TypeId == 0 ? null : Lookups.HivMentalSubstance[record.TypeId].Description);
			csv.WriteField(record.AdultsNo);
			csv.WriteField(record.ChildrenNo);
		}

		protected override IEnumerable<ClientInformationAggregateLineItem> PerformSelect(IQueryable<HivMentalSubstance> query) {
			return query.Select(m => new ClientInformationAggregateLineItem {
				Id = m.ID,
				CenterId = m.Center.CenterID,
				CenterName = m.Center.CenterName,
				TypeId = m.TypeID,
				AdultsNo = m.AdultsNo,
				ChildrenNo = m.ChildrenNo
			});
		}

		protected override void CreateReportTables() {
			var hivAidsGroup = new HivAidsReportTable("HIV/AIDS", 1) {
				Headers = GetNewAndOngoingHeaders(),
				HideSubheaders = true
			};
			foreach (var center in ReportContainer.Centers)
				hivAidsGroup.Rows.Add(new ReportRow { Title = center.CenterName, Code = center.CenterId });
			ReportTableList.Add(hivAidsGroup);

			var mentalHealthGroup = new MentalHealthReportTable("Mental Health", 2) {
				Headers = GetNewAndOngoingHeaders(),
				HideSubheaders = true
			};
			foreach (var center in ReportContainer.Centers)
				mentalHealthGroup.Rows.Add(new ReportRow { Title = center.CenterName, Code = center.CenterId });
			ReportTableList.Add(mentalHealthGroup);

			var substanceAbuseGroup = new SubstanceAbuseReportTable("Substance Abuse", 3) {
				Headers = GetNewAndOngoingHeaders(),
				HideSubheaders = true
			};
			foreach (var center in ReportContainer.Centers)
				substanceAbuseGroup.Rows.Add(new ReportRow { Title = center.CenterName, Code = center.CenterId });
			ReportTableList.Add(substanceAbuseGroup);
		}

		private List<ReportTableSubHeader> GetSubHeaders() {
			return new List<ReportTableSubHeader> {
				new ReportTableSubHeader { Title = "Total", Code = ReportTableSubHeaderEnum.Total }
			};
		}

		private new List<ReportTableHeader> GetNewAndOngoingHeaders() {
			var subHeaders = GetSubHeaders();
			return new List<ReportTableHeader> {
				new ReportTableHeader {
					Title = "Adult Counts",
					Code = ReportTableHeaderEnum.HIVAdultCount,
					SubHeaders = subHeaders
				},
				new ReportTableHeader {
					Title = "Child Counts",
					Code = ReportTableHeaderEnum.HIVChildCount,
					SubHeaders = subHeaders
				}
			};
		}
	}

	public class ClientInformationAggregateLineItem {
		public int? Id { get; set; }
		public int? CenterId { get; set; }
		public string CenterName { get; set; }
		public int TypeId { get; set; }
		public int? AdultsNo { get; set; }
		public int? ChildrenNo { get; set; }
	}
}