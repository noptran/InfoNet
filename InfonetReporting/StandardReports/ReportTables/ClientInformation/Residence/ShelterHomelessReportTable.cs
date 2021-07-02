using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Residence {
	public class ShelterHomelessReportTable : ReportTable<ClientInformationResidenceLineItem> {
		public ShelterHomelessReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationResidenceLineItem item) {
			if (item.HasPreviousServiceUse) {
				double? shelterServiceTimeSpan = null;
				double? homelessServiceTimeSpan = null;

				if (item.PreviousServiceUse.PrevShelterDate.HasValue)
					shelterServiceTimeSpan = (item.MostRecentShelterBeginDate.Value - item.PreviousServiceUse.PrevShelterDate).Value.TotalDays;

				if (item.PreviousServiceUse.PrevServiceDate.HasValue)
					homelessServiceTimeSpan = (item.MostRecentShelterBeginDate.Value - item.PreviousServiceUse.PrevServiceDate).Value.TotalDays;

				foreach (var row in Rows) {
					bool itemFallsInShelterCategory;
					bool itemFallsInHomelessCategory;
					switch (row.Code) {
						case (int)ShelterUseEnum.From0to3MonthsAgo:
							itemFallsInShelterCategory = shelterServiceTimeSpan >= 0 && shelterServiceTimeSpan <= 90;
							itemFallsInHomelessCategory = homelessServiceTimeSpan >= 0 && homelessServiceTimeSpan <= 90;
							break;
						case (int)ShelterUseEnum.From4to6MonthsAgo:
							itemFallsInShelterCategory = shelterServiceTimeSpan >= 91 && shelterServiceTimeSpan <= 180;
							itemFallsInHomelessCategory = homelessServiceTimeSpan >= 91 && homelessServiceTimeSpan <= 180;
							break;
						case (int)ShelterUseEnum.From7to9MonthsAgo:
							itemFallsInShelterCategory = shelterServiceTimeSpan >= 181 && shelterServiceTimeSpan <= 270;
							itemFallsInHomelessCategory = homelessServiceTimeSpan >= 181 && homelessServiceTimeSpan <= 270;
							break;
						case (int)ShelterUseEnum.From10to12MonthsAgo:
							itemFallsInShelterCategory = shelterServiceTimeSpan >= 271 && shelterServiceTimeSpan <= 365;
							itemFallsInHomelessCategory = homelessServiceTimeSpan >= 271 && homelessServiceTimeSpan <= 365;
							break;
						default:
							itemFallsInShelterCategory = item.PreviousServiceUse.PrevShelterDate == null || shelterServiceTimeSpan < 0;
							itemFallsInHomelessCategory = item.PreviousServiceUse.PrevServiceDate == null || homelessServiceTimeSpan < 0;
							break;
					}
					if (itemFallsInShelterCategory || itemFallsInHomelessCategory)
						foreach (var header in Headers) {
							foreach (var subheader in header.SubHeaders)
								if (item.ClientTypeID == (int)subheader.Code || subheader.Code == ReportTableSubHeaderEnum.Total)
									switch (header.Code) {
										case ReportTableHeaderEnum.DVShelterUse:
											if (item.PreviousServiceUse.PrevShelterUseId == (int)ShortAnswerEnum.Yes && itemFallsInShelterCategory)
												row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
											break;
										case ReportTableHeaderEnum.HomelessServiceUse:
											if (item.PreviousServiceUse.PrevServiceUseId == (int)ShortAnswerEnum.Yes && itemFallsInHomelessCategory)
												row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
											break;
										case ReportTableHeaderEnum.Total:
											if (item.PreviousServiceUse.PrevServiceUseId == (int)ShortAnswerEnum.Yes && itemFallsInHomelessCategory || item.PreviousServiceUse.PrevShelterUseId == (int)ShortAnswerEnum.Yes && itemFallsInShelterCategory)
												row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
											break;
									}
						}
				}
			}
		}
	}
}