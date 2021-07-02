using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class AgeReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public AgeReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			foreach (var row in Rows) {
				bool fitsThisAgeGroup = false;
				if (item.AgeAtFirstContact <= 19)
					switch (item.AgeAtFirstContact) {
                        case -1:
                            if (row.Code == -1)
                                fitsThisAgeGroup = true;
                            break;
                        case 0:
						case 1:
							if (row.Code == 1)
								fitsThisAgeGroup = true;
							break;
						case 2:
						case 3:
							if (row.Code == 2)
								fitsThisAgeGroup = true;
							break;
						case 4:
						case 5:
							if (row.Code == 3)
								fitsThisAgeGroup = true;
							break;
						case 6:
						case 7:
							if (row.Code == 4)
								fitsThisAgeGroup = true;
							break;
						case 8:
						case 9:
							if (row.Code == 5)
								fitsThisAgeGroup = true;
							break;
						case 10:
						case 11:
							if (row.Code == 6)
								fitsThisAgeGroup = true;
							break;
						case 12:
						case 13:
							if (row.Code == 7)
								fitsThisAgeGroup = true;
							break;
						case 14:
						case 15:
							if (row.Code == 8)
								fitsThisAgeGroup = true;
							break;
						case 16:
						case 17:
							if (row.Code == 9)
								fitsThisAgeGroup = true;
							break;
						case 18:
						case 19:
							if (row.Code == 10)
								fitsThisAgeGroup = true;
							break;
						default:
							if (row.Code == null)
								fitsThisAgeGroup = true;
							break;
					}
				else if (item.AgeAtFirstContact >= 20 && item.AgeAtFirstContact <= 29 && row.Code == 11)
					fitsThisAgeGroup = true;
				else if (item.AgeAtFirstContact >= 30 && item.AgeAtFirstContact <= 39 && row.Code == 12)
					fitsThisAgeGroup = true;
				else if (item.AgeAtFirstContact >= 40 && item.AgeAtFirstContact <= 49 && row.Code == 13)
					fitsThisAgeGroup = true;
				else if (item.AgeAtFirstContact >= 50 && item.AgeAtFirstContact <= 59 && row.Code == 14)
					fitsThisAgeGroup = true;
				else if (item.AgeAtFirstContact >= 60 && item.AgeAtFirstContact <= 64 && row.Code == 15)
					fitsThisAgeGroup = true;
				else if (item.AgeAtFirstContact >= 65 && row.Code == 16)
					fitsThisAgeGroup = true;

				if (fitsThisAgeGroup)
					foreach (var newOrOngoing in Headers) // Check New vs. Ongoing - allow Total
						if (item.ClientStatus == newOrOngoing.Code || newOrOngoing.Code == ReportTableHeaderEnum.Total)
							foreach (var clientType in newOrOngoing.SubHeaders) // Check if Client Type matches
								if (item.ClientTypeID == (int)clientType.Code || clientType.Code == ReportTableSubHeaderEnum.Total)
									row.Counts[newOrOngoing.Code.ToString()][clientType.Code.ToString()] += 1;
			}
		}
	}
}