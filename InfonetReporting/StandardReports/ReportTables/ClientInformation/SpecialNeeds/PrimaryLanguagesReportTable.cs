using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.SpecialNeeds {
	public class PrimaryLanguagesReportTable : ReportTable<ClientInformationSpecialNeedsLineItem> {
		public PrimaryLanguagesReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationSpecialNeedsLineItem item) {
			if (item.LimitedEnglish ?? false) {
				if (Rows.All(r => r.Code != item.PrimaryLanguageID))
					Rows.Add(new ReportRow { Code = item.PrimaryLanguageID, Title = Lookups.Language[item.PrimaryLanguageID]?.Description ?? "Unassigned", Counts = GetBlankDictionary(Headers) });

				foreach (var row in Rows)
					if (row.Code == item.PrimaryLanguageID)
						foreach (var header in Headers)
							if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
								foreach (var subheader in header.SubHeaders)
									if ((int)subheader.Code == item.ClientTypeID || subheader.Code == ReportTableSubHeaderEnum.Total)
										row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
			}
		}
	}
}