﻿using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.DirectServices {
	public class DirectServiceReportTable : ReportTable<DirectServiceLineItem> {
		private readonly Dictionary<int?, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _clientIdsByType = new Dictionary<int?, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();
		private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _uniqueClientsByType = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();
		private ISet<int?> _fundingSourceIds = null;
		private ISet<int?> _svIds = null;

		public DirectServiceReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> UniqueClientsByType {
			get { return _uniqueClientsByType; }
		}

		public IEnumerable<int?> FundingSourceIds {
			get { return _fundingSourceIds; }
			set { _fundingSourceIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public IEnumerable<int?> SvIds {
			get { return _svIds; }
			set { _svIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public override void PreCheckAndApply(ReportContainer container) {
			foreach (var header in Headers) {
				var innerDict = new Dictionary<ReportTableSubHeaderEnum, HashSet<int>>();
				foreach (var subheader in header.SubHeaders)
					innerDict.Add(subheader.Code, new HashSet<int>());
				_uniqueClientsByType.Add(header.Code, innerDict);
			}
			foreach (var row in Rows) {
				var innerDict = new Dictionary<ReportTableSubHeaderEnum, HashSet<int>>();
				foreach (var subheader in Headers.First().SubHeaders)
					innerDict.Add(subheader.Code, new HashSet<int>());
				_clientIdsByType.Add(row.Code, innerDict);
			}
		}

		public override void CheckAndApply(DirectServiceLineItem item) {
			double averagePercentFundedPerStaff = 1;
			if (_fundingSourceIds != null) {
				int staffCount = item.StaffAndFunding.Select(sf => sf.SvId).Distinct().Count();
				int percentFundedSum = item.StaffAndFunding.Where(sf => sf.FundingSourceId != null && _fundingSourceIds.Contains(sf.FundingSourceId) && (_svIds?.Contains(sf.SvId) ?? true)).Sum(sf => sf.PercentFund ?? 0);
				averagePercentFundedPerStaff = percentFundedSum / 100.0 / staffCount;
			}

			foreach (var row in Rows.Where(r => r.Code == item.ServiceId))
				foreach (var eachHeader in Headers) {
					foreach (var eachClientType in eachHeader.SubHeaders.Where(ct => (int)ct.Code == item.ClientTypeId || ct.Code == ReportTableSubHeaderEnum.Total))
						switch (eachHeader.Code) {
							case ReportTableHeaderEnum.NumberOfClientsReceivingServices:
								var clientIdsForRowAndType = _clientIdsByType[row.Code][eachClientType.Code];
								clientIdsForRowAndType.Add(item.ClientId.Value);
								row.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] = clientIdsForRowAndType.Count;

								var uniqueClientsForHeaderAndType = _uniqueClientsByType[eachHeader.Code][eachClientType.Code];
								uniqueClientsForHeaderAndType.Add(item.ClientId.Value);
								NonDuplicatedSubtotalRow.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] = uniqueClientsForHeaderAndType.Count;
								break;
							case ReportTableHeaderEnum.NumberOfContacts:
								row.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] += 1;
								NonDuplicatedSubtotalRow.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] += 1;
								break;
							case ReportTableHeaderEnum.HoursOfService:
								row.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] += item.ReceivedHours * averagePercentFundedPerStaff ?? 0.0;
								NonDuplicatedSubtotalRow.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] += item.ReceivedHours * averagePercentFundedPerStaff ?? 0.0;
								break;
						}
				}
		}
	}
}