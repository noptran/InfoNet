using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.NonClientCrisisIntervention {
	public class CrisisInterventionAgeReportTable : ReportTable<CrisisInterventionLineItem> {
        public CrisisInterventionAgeReportTable(string title, int displayOrder) : base(title, displayOrder) {

        }
        public override void CheckAndApply(CrisisInterventionLineItem item) {
            ReportTableHeaderEnum callType = item.CallTypeId == (int)CrisisInterventionCallTypeEnum.InPerson ? ReportTableHeaderEnum.InPersonContacts : ReportTableHeaderEnum.CrisisInterventionPhoneContacts;
            foreach (ReportRow row in Rows) {
                bool fitsThisAgeGroup = false;
                switch (row.Code) {
                    case (int)AgeRangeEnum.ZeroToSeven:
                        fitsThisAgeGroup = item.Age >= 0 && item.Age <= 7;
                        break;
                    case (int)AgeRangeEnum.EightToNine:
                        fitsThisAgeGroup = item.Age == 8 || item.Age == 9;
                        break;
                    case (int)AgeRangeEnum.TenToEleven:
                        fitsThisAgeGroup = item.Age == 10 || item.Age == 11;
                        break;
                    case (int)AgeRangeEnum.TwelveToThirteen:
                        fitsThisAgeGroup = item.Age == 12 || item.Age == 13;
                        break;
                    case (int)AgeRangeEnum.FourteenToFifteen:
                        fitsThisAgeGroup = item.Age == 14 || item.Age == 15;
                        break;
                    case (int)AgeRangeEnum.SixteenToSeventeen:
                        fitsThisAgeGroup = item.Age == 16 || item.Age == 17;
                        break;
                    case (int)AgeRangeEnum.EighteenToNineteen:
                        fitsThisAgeGroup = item.Age == 18 || item.Age == 19;
                        break;
                    case (int)AgeRangeEnum.Twenties:
                        fitsThisAgeGroup = item.Age >= 20 && item.Age <= 29;
                        break;
                    case (int)AgeRangeEnum.Thirties:
                        fitsThisAgeGroup = item.Age >= 30 && item.Age <= 39;
                        break;
                    case (int)AgeRangeEnum.Fourties:
                        fitsThisAgeGroup = item.Age >= 40 && item.Age <= 49;
                        break;
                    case (int)AgeRangeEnum.Fifties:
                        fitsThisAgeGroup = item.Age >= 50 && item.Age <= 59;
                        break;
                    case (int)AgeRangeEnum.SixtyToSixtyFour:
                        fitsThisAgeGroup = item.Age >= 60 && item.Age <= 64;
                        break;
                    case (int)AgeRangeEnum.SixtyFiveAndUp:
                        fitsThisAgeGroup = item.Age >= 65;
                        break;
                    case (int)AgeRangeEnum.Unknown:
                        fitsThisAgeGroup = item.Age == -1;
                        break;
                    case (int)AgeRangeEnum.Unassigned:
                        fitsThisAgeGroup = item.Age == null;
                        break;
                }

                if (fitsThisAgeGroup) {
                    foreach (ReportTableHeader header in Headers) {
                        if (callType == header.Code || header.Code == ReportTableHeaderEnum.Total) {
                            row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += item.NumberOfContacts ?? 0;
                        }
                    }
                }
            }
        }
    }

    internal enum AgeRangeEnum {
        ZeroToSeven,
        EightToNine,
        TenToEleven,
        TwelveToThirteen,
        FourteenToFifteen,
        SixteenToSeventeen,
        EighteenToNineteen,
        Twenties,
        Thirties,
        Fourties,
        Fifties,
        SixtyToSixtyFour,
        SixtyFiveAndUp,
        Unknown,
        Unassigned,
    }
}