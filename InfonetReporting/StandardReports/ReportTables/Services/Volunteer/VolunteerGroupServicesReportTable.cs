using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.Volunteer {
	public class VolunteerGroupServicesReportTable : ReportTable<GroupStaffLineItem> {
		private readonly HashSet<int> _uniqueIcsIds = new HashSet<int>();
		private ISet<int?> _fundingSourceIds = null;

		public VolunteerGroupServicesReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public IEnumerable<int?> FundingSourceIds {
			get { return _fundingSourceIds; }
			set { _fundingSourceIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public override void CheckAndApply(GroupStaffLineItem item) {
			double percentFunded = 1;
			if (_fundingSourceIds != null)
				percentFunded = item.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0);

			foreach (var row in Rows)
				switch ((ReportTableHeaderEnum)row.Code) {
					case ReportTableHeaderEnum.StaffConductHours:
						row.Counts[TotalHeader][TotalSubHeader] += item.StaffConductHours * percentFunded;
						break;
					case ReportTableHeaderEnum.StaffPreparationHours:
						row.Counts[TotalHeader][TotalSubHeader] += item.StaffPrepHours * percentFunded;
						break;
					case ReportTableHeaderEnum.StaffTravelHours:
						row.Counts[TotalHeader][TotalSubHeader] += item.StaffTravelHours * percentFunded;
						break;
					case ReportTableHeaderEnum.Total:
						row.Counts[TotalHeader][TotalSubHeader] += (item.StaffConductHours + item.StaffPrepHours + item.StaffTravelHours) * percentFunded;
						break;
					case ReportTableHeaderEnum.NumberOfContacts:
						if (_uniqueIcsIds.Add(item.IcsId))
							row.Counts[TotalHeader][TotalSubHeader] += 1;
						break;
				}
		}
	}
}