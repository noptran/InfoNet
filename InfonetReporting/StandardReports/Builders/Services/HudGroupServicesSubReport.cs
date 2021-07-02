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
using Infonet.Reporting.StandardReports.ReportTables.Services.Hud;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class HudGroupServicesSubReport : SubReportCountBuilder<ProgramDetail, HudGroupServiceLineItem> {
		private HashSet<int?> _fundingSourceIds = null;

		public HudGroupServicesSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<ProgramDetailFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
		}

		protected override IEnumerable<HudGroupServiceLineItem> PerformSelect(IQueryable<ProgramDetail> query) {
			var staffPredicate = PredicateBuilder.New<ProgramDetailStaff>(true);
			var svIds = ReportQuery.Filters.OfType<ProgramDetailStaffFilter>().SingleOrDefault()?.SvIds;
			if (svIds != null)
				staffPredicate.And(pds => svIds.Contains(pds.SVID));

			return query.Select(q => new HudGroupServiceLineItem {
				IcsId = q.ICS_ID,
				Center = q.Center.CenterName,
				ServiceDate = q.PDate,
				HudServices = q.TLU_Codes_ProgramsAndServices.HudServices.Select(hs => hs.HudServiceId),
				NumberOfPresentations = q.NumOfSession ?? 0,
				NumberOfParticipants = q.ParticipantsNum ?? 0,
				PresentationHours = q.Hours ?? 0,
				Staff = q.ProgramDetailStaff.AsQueryable().Where(staffPredicate).Select(pds => new StaffLineItem {
					SvId = pds.SVID,
					ConductHours = pds.ConductHours ?? 0,
					PrepHours = pds.HoursPrep ?? 0,
					TravelHours = pds.HoursTravel ?? 0,
					Funding = StaffFunding.ProgramDetailStaff.Invoke(pds)
				})
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Date", "HUD Service(s)", "Number of Presentations", "Number of Participants", "Number of Staff", "Presentation Hours", "Staff Conduct Hours", "Staff Travel Hours", "Staff Prep Hours" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, HudGroupServiceLineItem record) {
			csv.WriteField(record.IcsId);
			csv.WriteField(record.Center);
			csv.WriteField(record.ServiceDate, "M/d/yyyy");
			csv.WriteField(string.Join("|", record.HudServices.Select(hs => Lookups.HudGroupServices[hs]).OrderBy(lc => lc.Entries[ReportContainer.Provider]).Select(lc => lc.Description)));
			csv.WriteField(record.NumberOfPresentations);
			csv.WriteField(record.NumberOfParticipants);
			csv.WriteField(record.Staff.Select(s => s.SvId).Distinct().Count());
			csv.WriteField(record.PresentationHours);
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.ConductHours)
				: record.Staff.Sum(s => s.ConductHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.TravelHours)
				: record.Staff.Sum(s => s.TravelHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.PrepHours)
				: record.Staff.Sum(s => s.PrepHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
		}

		protected override void CreateReportTables() {
			var groupServices = new HudGroupServicesReportTable("Group Services", 1) {
				Headers = CommunityGroupSubReport.GetHeaders(),
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true,
				FundingSourceIds = _fundingSourceIds
			};
			foreach (var item in Lookups.HudGroupServices[ReportContainer.Provider])
				groupServices.Rows.Add(new ReportRow { Code = item.CodeId, Title = item.Description, Order = item.Entries[ReportContainer.Provider].DisplayOrder });
			ReportTableList.Add(groupServices);
		}
	}

	public class HudGroupServiceLineItem {
		public int? IcsId { get; set; }
		public string Center { get; set; }
		public DateTime? ServiceDate { get; set; }
		public IEnumerable<int> HudServices { get; set; }
		public int NumberOfPresentations { get; set; }
		public int NumberOfParticipants { get; set; }
		public double? PresentationHours { get; set; }
		public IEnumerable<StaffLineItem> Staff { get; set; }
	}
}