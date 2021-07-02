using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.StaffService {
	public class RaceHudReportTable : ReportTable<ManagementClientInformationDemographicsLineItem> {
		public RaceHudReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ManagementClientInformationDemographicsLineItem item) {
			if (item.ClientStatus == ReportTableHeaderEnum.New) {
				foreach (var row in Rows) {
					if (Provider == Data.Looking.Provider.CAC) {
						if (row.Code == item.RaceId) {
							foreach (var header in Headers) {
								if (item.Gender == header.Code || header.Code == ReportTableHeaderEnum.Total) {
									foreach (var subheader in header.SubHeaders) {
										row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
									}
								}
							}
						}
					} else {
						if (row.Code < 90 && item.RaceIDs.Contains(row.Code ?? 0)) {
							foreach (var header in Headers) {
								if (item.Gender == header.Code || header.Code == ReportTableHeaderEnum.Total) {
									foreach (var subheader in header.SubHeaders) {
										row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
									}
								}
							}
						} else if (row.Code > 90) {
							bool hasComposite = false;
							switch (row.Code) {
								case (int)RaceHudCompositeEnum.AmericanIndianOrAlaskaNativeAndBlackOrAfricanAmerican:
									hasComposite = item.RaceIDs.Contains((int)RaceHudEnum.AmericanIndian) && item.RaceIDs.Contains((int)RaceHudEnum.Black);
									break;
								case (int)RaceHudCompositeEnum.AmericanIndianOrAlaskaNativeAndWhite:
									hasComposite = item.RaceIDs.Contains((int)RaceHudEnum.AmericanIndian) && item.RaceIDs.Contains((int)RaceHudEnum.White);
									break;
								case (int)RaceHudCompositeEnum.AsianAndWhite:
									hasComposite = item.RaceIDs.Contains((int)RaceHudEnum.Asian) && item.RaceIDs.Contains((int)RaceHudEnum.White);
									break;
								case (int)RaceHudCompositeEnum.BlackOrAfricanAmericanAndWhite:
									hasComposite = item.RaceIDs.Contains((int)RaceHudEnum.Black) && item.RaceIDs.Contains((int)RaceHudEnum.White);
									break;
							}
							if (hasComposite) {
								foreach (var header in Headers) {
									if (item.Gender == header.Code || header.Code == ReportTableHeaderEnum.Total) {
										foreach (var subheader in header.SubHeaders) {
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										}
									}
								}
							}
						}
					}
				}
			}

		}
	}
}