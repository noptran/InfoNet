using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.StaffService {
	public class EthnicityReportTable : ReportTable<ManagementClientInformationDemographicsLineItem> {
		private readonly HashSet<int?> _clientIds;

		public EthnicityReportTable(string title, int displayOrder) : base(title, displayOrder) {
			_clientIds = new HashSet<int?>();
		}

		public override void CheckAndApply(ManagementClientInformationDemographicsLineItem item) {
			if (item.ClientStatus == ReportTableHeaderEnum.New)
				if (!_clientIds.Contains(item.ClientID))
					foreach (var row in Rows)
						/* Second part of the if statement explanation 
						 * We are using ethnicityID to determine current client's ethnicity which is coming from TLU_Codes_Ethnicity table
						 * The above will not cover the ones that are NULL 
						 * the NULLs are in unknown (codeid of 3) category 
						 */
						if (row.Code == item.EthnicityID || row.Code == 3 && !item.EthnicityID.HasValue)
							foreach (var header in Headers) // Check Male vs. Female - allow Total
								if (item.Gender == header.Code || header.Code == ReportTableHeaderEnum.Total)
									foreach (var subheader in header.SubHeaders) {
										row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
										_clientIds.Add(item.ClientID);
									}
		}
	}
}