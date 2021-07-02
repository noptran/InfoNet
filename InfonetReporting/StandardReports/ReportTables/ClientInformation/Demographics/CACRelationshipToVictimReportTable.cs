using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class CACRelationshipToVictimReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public CACRelationshipToVictimReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			if (new[] { (int?)ReportTableSubHeaderEnum.CACSignifigantOther, (int?)ReportTableSubHeaderEnum.ChildNonVictim, (int?)ReportTableSubHeaderEnum.NonOffendingCaretaker }.Contains(item.ClientTypeID)) {
				if (Rows.All(r => r.Code != item.RelationshipToVictimID)) {
					var newRow = new ReportRow { Order = item.RelationshipToVictimID == null ? 999999 : Lookups.RelationshipToClient[Provider.CAC][item.RelationshipToVictimID].Entries[Provider.CAC].DisplayOrder, Code = item.RelationshipToVictimID, Title = item.RelationshipToVictimID == null ? "Unassigned" : Lookups.RelationshipToClient[item.RelationshipToVictimID]?.Description, Counts = GetBlankDictionary(Headers) };
					foreach (var header in Headers)
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders)
								if ((int)subheader.Code == item.ClientTypeID || subheader.Code == ReportTableSubHeaderEnum.Total)
									newRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
					Rows.Add(newRow);
				} else {
					foreach (var row in Rows)
						if (row.Code == item.RelationshipToVictimID)
							foreach (var header in Headers)
								if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
									foreach (var subheader in header.SubHeaders)
										if ((int)subheader.Code == item.ClientTypeID || subheader.Code == ReportTableSubHeaderEnum.Total)
											row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
				}
			}
		}
	}
}