using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.DirectServices {
	public class ReferralsReportTable : ReportTable<ReferralLineItem> {
		private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _uniqueClientsByType = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();
		private readonly Dictionary<int?, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _clientIdsByType = new Dictionary<int?, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();


        public ReferralsReportTable(string title, int displayOrder) : base(title, displayOrder) { }

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

		public override void CheckAndApply(ReferralLineItem item) {
			foreach (var row in Rows.Where(r => item.ReferralTypeID == r.Code))
				foreach (var eachHeader in Headers) {
					foreach (var eachClientType in eachHeader.SubHeaders) {
						row.Counts[ReportTableHeaderEnum.HoursOfService.ToString()][eachClientType.Code.ToString()] = (int)ReportStringOutputEnum.NA;
						if ((int)eachClientType.Code == item.ClientTypeId || eachClientType.Code == ReportTableSubHeaderEnum.Total)
							switch (eachHeader.Code) {
								case ReportTableHeaderEnum.NumberOfClientsReceivingServices:
									var clientIdsForRowAndType = _clientIdsByType[row.Code][eachClientType.Code];
									clientIdsForRowAndType.Add(item.ClientID.Value);
									row.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] = clientIdsForRowAndType.Count;

									var uniqueClientsForHeaderAndType = _uniqueClientsByType[eachHeader.Code][eachClientType.Code];
									uniqueClientsForHeaderAndType.Add(item.ClientID.Value);
									NonDuplicatedSubtotalRow.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] = uniqueClientsForHeaderAndType.Count;
									break;
								case ReportTableHeaderEnum.NumberOfContacts:
									row.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] += 1;
									NonDuplicatedSubtotalRow.Counts[eachHeader.Code.ToString()][eachClientType.Code.ToString()] += 1;
									break;
							}
					}
				}
		}
    }
}