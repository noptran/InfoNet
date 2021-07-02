using System.Collections.Generic;
using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class RaceHudReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		private readonly Dictionary<string, Dictionary<string, HashSet<int?>>> _clientIdsByType = new Dictionary<string, Dictionary<string, HashSet<int?>>>();
		private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<int?, HashSet<int?>>>> _uniqueClients = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, Dictionary<int?, HashSet<int?>>>>();

		public RaceHudReportTable(string title, int displayOrder) : base(title, displayOrder) { }
      

		public override void PreCheckAndApply(ReportContainer container) {
			foreach (var header in Headers) {
				var innerDict = new Dictionary<ReportTableSubHeaderEnum, Dictionary<int?, HashSet<int?>>>();
				foreach (var subheader in header.SubHeaders) {
					var rowDict = new Dictionary<int?, HashSet<int?>>();
					foreach (var row in Rows)
						rowDict.Add(row.Code, new HashSet<int?>());
					innerDict.Add(subheader.Code, rowDict);
				}
				_uniqueClients.Add(header.Code, innerDict);
			}
		}

		// PRC DO Revise calculations
		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			foreach (var row in Rows)
				if (row.Code < 90 && item.RaceIDs.Contains(row.Code ?? 0)) {
					foreach (var header in Headers) // Check New vs. Ongoing - allow Total
						if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders) {
								var currentSet = _uniqueClients[header.Code][subheader.Code][row.Code];
								// Check if Client Type matches
								if ((item.ClientTypeID == (int)subheader.Code || subheader.Code == ReportTableSubHeaderEnum.Total) && currentSet.Add(item.ClientID)) {
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] = currentSet.Count;
									SetNonDuplicatedRow(header.Code, subheader.Code, item.ClientID);
								}
							}
				} else if (row.Code > 90) {
					bool hasComposite = false;
                    if (row.Code == (int)RaceHudCompositeEnum.MENAORWhite) //Total for either MENA or Whilte Clients
                        hasComposite = item.RaceIDs.Contains((int)RaceHudEnum.MENA) || item.RaceIDs.Contains((int)RaceHudEnum.White);
                    else if (row.Code == (int)RaceHudCompositeEnum.AsianORSouthAsian) //Total for either Asian or South Asian
                        hasComposite = item.RaceIDs.Contains((int)RaceHudEnum.Asian) || item.RaceIDs.Contains((int)RaceHudEnum.SouthAsian);
                    else if (item.RaceIDs.Count() > 1)
                        switch (row.Code) {
                            case (int)RaceHudCompositeEnum.AmericanIndianOrAlaskaNativeAndBlackOrAfricanAmerican:
                                hasComposite = item.RaceIDs.Contains((int)RaceHudEnum.AmericanIndian) && item.RaceIDs.Contains((int)RaceHudEnum.Black);
                                break;
                            case (int)RaceHudCompositeEnum.AmericanIndianOrAlaskaNativeAndWhite:
                                hasComposite = item.RaceIDs.Contains((int)RaceHudEnum.AmericanIndian) && item.RaceIDs.Contains((int)RaceHudEnum.White);
                                break;
                            case (int)RaceHudCompositeEnum.AsianAndWhite:
                                hasComposite =
                                   item.RaceIDs.Contains((int)RaceHudEnum.Asian) && item.RaceIDs.Contains((int)RaceHudEnum.White) ||
                                   item.RaceIDs.Contains((int)RaceHudEnum.SouthAsian) && item.RaceIDs.Contains((int)RaceHudEnum.White);
                                break;
                            case (int)RaceHudCompositeEnum.BlackOrAfricanAmericanAndWhite:
                                hasComposite = item.RaceIDs.Contains((int)RaceHudEnum.Black) && item.RaceIDs.Contains((int)RaceHudEnum.White);
                                break;
                            case (int)RaceHudCompositeEnum.OtherMultiracial: //Excludes Hispanic/Latino + White; MENA + White; and Asian + South Asian
                                hasComposite =
                                    item.RaceIDs.Contains((int)RaceHudEnum.AmericanIndian) && item.RaceIDs.Contains((int)RaceHudEnum.Asian) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.AmericanIndian) && item.RaceIDs.Contains((int)RaceHudEnum.SouthAsian) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.AmericanIndian) && item.RaceIDs.Contains((int)RaceHudEnum.HispanicLatino) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.AmericanIndian) && item.RaceIDs.Contains((int)RaceHudEnum.NativeHawaiian) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.Asian) && item.RaceIDs.Contains((int)RaceHudEnum.Black) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.Asian) && item.RaceIDs.Contains((int)RaceHudEnum.HispanicLatino) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.Asian) && item.RaceIDs.Contains((int)RaceHudEnum.NativeHawaiian) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.SouthAsian) && item.RaceIDs.Contains((int)RaceHudEnum.Black) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.SouthAsian) && item.RaceIDs.Contains((int)RaceHudEnum.HispanicLatino) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.SouthAsian) && item.RaceIDs.Contains((int)RaceHudEnum.NativeHawaiian) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.Black) && item.RaceIDs.Contains((int)RaceHudEnum.HispanicLatino) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.Black) && item.RaceIDs.Contains((int)RaceHudEnum.NativeHawaiian) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.HispanicLatino) && item.RaceIDs.Contains((int)RaceHudEnum.NativeHawaiian) ||                                  
                                    item.RaceIDs.Contains((int)RaceHudEnum.NativeHawaiian) && item.RaceIDs.Contains((int)RaceHudEnum.White) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.MENA) && item.RaceIDs.Contains((int)RaceHudEnum.AmericanIndian) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.MENA) && item.RaceIDs.Contains((int)RaceHudEnum.Asian) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.MENA) && item.RaceIDs.Contains((int)RaceHudEnum.SouthAsian) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.MENA) && item.RaceIDs.Contains((int)RaceHudEnum.Black) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.MENA) && item.RaceIDs.Contains((int)RaceHudEnum.HispanicLatino) ||
                                    item.RaceIDs.Contains((int)RaceHudEnum.MENA) && item.RaceIDs.Contains((int)RaceHudEnum.Black);
                                break;
                        }
                    
					if (hasComposite)
						foreach (var header in Headers) // Check New vs. Ongoing - allow Total
							if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
								foreach (var subheader in header.SubHeaders) {
									var currentSet = _uniqueClients[header.Code][subheader.Code][row.Code];
									// Check if Client Type matches
									if ((item.ClientTypeID == (int)subheader.Code || subheader.Code == ReportTableSubHeaderEnum.Total) && currentSet.Add(item.ClientID))
										row.Counts[header.Code.ToString()][subheader.Code.ToString()] = currentSet.Count;
								}
				}
		}

		private void SetNonDuplicatedRow(ReportTableHeaderEnum header, ReportTableSubHeaderEnum subheader, int? clientId) {
			Dictionary<string, HashSet<int?>> innerDict;
			bool outerExists = _clientIdsByType.TryGetValue(header.ToString(), out innerDict);
			if (outerExists) {
				HashSet<int?> clientIdList;
				bool innerExists = innerDict.TryGetValue(subheader.ToString(), out clientIdList);
				if (innerExists) {
					clientIdList.Add(clientId);
				} else {
					clientIdList = new HashSet<int?>();
					clientIdList.Add(clientId);
					innerDict.Add(subheader.ToString(), clientIdList);
				}
			} else {
				var clientIdList = new HashSet<int?>();
				clientIdList.Add(clientId);
				innerDict = new Dictionary<string, HashSet<int?>>();
				innerDict.Add(subheader.ToString(), clientIdList);
				_clientIdsByType.Add(header.ToString(), innerDict);
			}
			NonDuplicatedSubtotalRow.Counts[header.ToString()][subheader.ToString()] = _clientIdsByType[header.ToString()][subheader.ToString()].Count;
		}
	}
}