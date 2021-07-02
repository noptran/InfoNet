using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.Offender {
	public class MedicalCJOffendersAgeReportTable : ReportTable<MedicalCJOffendersLineItem> {
		public MedicalCJOffendersAgeReportTable(string title, int displayOrder) : base(title, displayOrder) {
			OffenderCases = new HashSet<string>();
		}

		private HashSet<string> OffenderCases { get; }

		public override void CheckAndApply(MedicalCJOffendersLineItem item) {
			string offenserCaseIdentifier = $"{item.OffenderID}:{item.ClientID}:{item.CaseID}";
			if (!OffenderCases.Contains(offenserCaseIdentifier))
				foreach (ReportRow row in Rows) {
					bool fitsThisAgeGroup = false;
					switch (row.Code) {
						case (int)MedicalCJOffendersAgeRangeEnum.ZeroToFifteen:
							fitsThisAgeGroup = item.Age >= 0 && item.Age <= 15;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.SixteenToSeventeen:
							fitsThisAgeGroup = item.Age == 16 || item.Age == 17;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.EighteenToNineteen:
							fitsThisAgeGroup = item.Age == 18 || item.Age == 19;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.Twenties:
							fitsThisAgeGroup = item.Age >= 20 && item.Age <= 29;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.Thirties:
							fitsThisAgeGroup = item.Age >= 30 && item.Age <= 39;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.Fourties:
							fitsThisAgeGroup = item.Age >= 40 && item.Age <= 49;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.Fifties:
							fitsThisAgeGroup = item.Age >= 50 && item.Age <= 59;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.SixtyToSixtyFour:
							fitsThisAgeGroup = item.Age >= 60 && item.Age <= 64;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.SixtyFiveAndUp:
							fitsThisAgeGroup = item.Age >= 65;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.Unknown:
							fitsThisAgeGroup = item.Age == -1;
							break;
						case (int)MedicalCJOffendersAgeRangeEnum.Unassigned:
							fitsThisAgeGroup = item.Age == null;
							break;
					}
					if (fitsThisAgeGroup)
						foreach (ReportTableHeader newOrOngoing in Headers) // Check New vs. Ongoing - allow Total
							if (item.ClientStatus == newOrOngoing.Code || newOrOngoing.Code == ReportTableHeaderEnum.Total)
								foreach (ReportTableSubHeader clientType in newOrOngoing.SubHeaders) {
									row.Counts[newOrOngoing.Code.ToString()][clientType.Code.ToString()] += 1;
									OffenderCases.Add(offenserCaseIdentifier);
								}
				}
		}
	}

	internal enum MedicalCJOffendersAgeRangeEnum {
		ZeroToFifteen = 1,
		SixteenToSeventeen = 2,
		EighteenToNineteen = 4,
		Twenties = 5,
		Thirties = 6,
		Fourties = 7,
		Fifties = 8,
		SixtyToSixtyFour = 9,
		SixtyFiveAndUp = 10,
		Unknown = 11,
		Unassigned = 12
	}
}