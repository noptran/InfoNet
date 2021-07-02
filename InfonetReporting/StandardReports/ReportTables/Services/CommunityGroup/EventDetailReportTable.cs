using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.CommunityGroup {
	public class EventDetailReportTable : ReportTable<EventDetailLineItem> {
		private readonly Dictionary<int?, HashSet<int>> _uniqueStaffLists = new Dictionary<int?, HashSet<int>>();
		private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _uniqueStaffByType = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();
		private ISet<int?> _fundingSourceIds = null;

		public EventDetailReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public IEnumerable<int?> FundingSourceIds {
			get { return _fundingSourceIds; }
			set { _fundingSourceIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public override void PreCheckAndApply(ReportContainer container) {
			foreach (var header in Headers) {
				var innerDict = new Dictionary<ReportTableSubHeaderEnum, HashSet<int>>();
				foreach (var subheader in header.SubHeaders)
					innerDict.Add(subheader.Code, new HashSet<int>());
				_uniqueStaffByType.Add(header.Code, innerDict);
			}
		}

		public override void CheckAndApply(EventDetailLineItem item) {
			var svIds = item.Staff.Select(s => s.SvId).ToArray();
			foreach (var row in Rows.Where(r => r.Code == item.ProgramId)) {
				CheckStaffLists(row.Code, svIds);
				foreach (var header in Headers) {
					foreach (var subheader in header.SubHeaders)
						switch (header.Code) {
							case ReportTableHeaderEnum.EventNumberOfEvents:
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
								break;
							case ReportTableHeaderEnum.EventNumberOfPeopleReached:
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.NumOfPeopleReached ?? 0;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.NumOfPeopleReached ?? 0;
								break;
							case ReportTableHeaderEnum.NumberOfStaff:
								foreach (var eachSet in _uniqueStaffLists.Values)
									_uniqueStaffByType[header.Code][subheader.Code].AddRange(eachSet);
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] = _uniqueStaffLists[row.Code].Count;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] = _uniqueStaffByType[header.Code][subheader.Code].Count;
								break;
							case ReportTableHeaderEnum.EventHours:
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.EventHours ?? 0;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.EventHours ?? 0;
								break;
							case ReportTableHeaderEnum.StaffConductHours:
								double conductHours = _fundingSourceIds == null
									? item.Staff.Sum(s => s.ConductHours)
									: item.Staff.Sum(s => s.ConductHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0));
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += conductHours;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += conductHours;
								break;
							case ReportTableHeaderEnum.StaffTravelHours:
								double travelHours = _fundingSourceIds == null
									? item.Staff.Sum(s => s.TravelHours)
									: item.Staff.Sum(s => s.TravelHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0));
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += travelHours;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += travelHours;
								break;
							case ReportTableHeaderEnum.StaffPreparationHours:
								double prepHours = _fundingSourceIds == null
									? item.Staff.Sum(s => s.PrepHours)
									: item.Staff.Sum(s => s.PrepHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0));
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += prepHours;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += prepHours;
								break;
						}
				}
			}
		}

		private void CheckStaffLists(int? code, IEnumerable<int> svids) {
			HashSet<int> svidList;
			if (_uniqueStaffLists.TryGetValue(code, out svidList))
				svidList.AddRange(svids);
			else
				_uniqueStaffLists.Add(code, new HashSet<int>(svids));
		}
	}
}