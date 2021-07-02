using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.StandardReports.ReportTables.Services.Volunteer;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class VolunteerGroupServicesSubReport : SubReportCountBuilder<ProgramDetailStaff, GroupStaffLineItem> {
		private HashSet<int?> _fundingSourceIds = null;

		public VolunteerGroupServicesSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
			IsEndOfGroup = true;
		}

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<ProgramDetailFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
		}

		protected override IEnumerable<GroupStaffLineItem> PerformSelect(IQueryable<ProgramDetailStaff> query) {
			return query.Select(pds => new GroupStaffLineItem {
					Id = (int)pds.ICS_Staff_ID,
					IcsId = (int)pds.ICS_ID,
					Center = pds.ProgramDetail.Center.CenterName,
					ProgramId = pds.ProgramDetail.ProgramID,
					ProgramDate = pds.ProgramDetail.PDate,
					Volunteer = (pds.StaffVolunteer.FirstName + " " + pds.StaffVolunteer.LastName).Trim(),
					StaffConductHours = pds.ConductHours ?? 0,
					StaffPrepHours = pds.HoursPrep ?? 0,
					StaffTravelHours = pds.HoursTravel ?? 0,
					Funding = StaffFunding.ProgramDetailStaff.Invoke(pds)
				}
			);
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "ICS ID", "Center", "Program Name", "Program Date", "Volunteer", "Staff Conduct Hours", "Staff Prep Hours", "Staff Travel Hours" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, GroupStaffLineItem record) {
			double percentFunded = 1;
			if (_fundingSourceIds != null)
				percentFunded = record.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0);

			csv.WriteField(record.Id);
			csv.WriteField(record.IcsId);
			csv.WriteField(record.Center);
			csv.WriteField(Lookups.ProgramsAndServices[record.ProgramId].Description);
			csv.WriteField(record.ProgramDate, "M/d/yyyy");
			csv.WriteField(record.Volunteer);
			csv.WriteField(record.StaffConductHours * percentFunded);
			csv.WriteField(record.StaffPrepHours * percentFunded);
			csv.WriteField(record.StaffTravelHours * percentFunded);
		}

		protected override void CreateReportTables() {
			var table = new VolunteerGroupServicesReportTable("Community, Institutional and Group Services", 3) {
				Headers = GetHeaders(),
				HideSubheaders = true,
				HideSubtotal = true,
				FundingSourceIds = _fundingSourceIds
			};
			table.Rows.Add(new ReportRow { Code = (int)ReportTableHeaderEnum.StaffConductHours, Title = "Presentation Hours", Order = 1 });
			table.Rows.Add(new ReportRow { Code = (int)ReportTableHeaderEnum.StaffPreparationHours, Title = "Preparation Hours", Order = 2 });
			table.Rows.Add(new ReportRow { Code = (int)ReportTableHeaderEnum.StaffTravelHours, Title = "Travel Hours", Order = 3 });
			table.Rows.Add(new ReportRow { Code = (int)ReportTableHeaderEnum.Total, Title = "Total Community and Inst Service Hours", Order = 4 });
			table.Rows.Add(new ReportRow { Code = (int)ReportTableHeaderEnum.NumberOfContacts, Title = "Number Of Contacts", Order = 5 });
			ReportTableList.Add(table);
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> { new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = string.Empty, SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } } };
		}
	}

	public class GroupStaffLineItem {
		public int Id { get; set; }
		public int IcsId { get; set; }
		public string Center { get; set; }
		public int ProgramId { get; set; }
		public DateTime? ProgramDate { get; set; }
		public string Volunteer { get; set; }
		public double StaffConductHours { get; set; }
		public double StaffTravelHours { get; set; }
		public double StaffPrepHours { get; set; }
		public IEnumerable<StaffFunding> Funding { get; set; }
	}
}