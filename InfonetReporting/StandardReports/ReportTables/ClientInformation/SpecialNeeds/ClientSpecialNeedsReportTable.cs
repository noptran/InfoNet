using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.SpecialNeeds {
	public class ClientSpecialNeedsReportTable : ReportTable<ClientInformationSpecialNeedsLineItem> {
		public ClientSpecialNeedsReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationSpecialNeedsLineItem item) {
			foreach (var row in Rows) {
				bool itemHasThisDisability = false;
				switch (row.Code) {
					case (int)DisabilityEnum.AssistanceADL:
						if (item.ADLProblem ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.DevelopmentalDisability:
						if (item.DevelopmentalDisabled ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.HearingImpairment:
						if (item.Deaf ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.Immobility:
						if (item.Immobile ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.LimitedEnglish:
						if (item.LimitedEnglish ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.MedicationAdministered:
						if (item.MedsAdministered ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.MentalEmotionalDisability:
						if (item.MentalDisability ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.NoSpecialNeedsIndicated:
						if (item.NoSpecialNeeds ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.NotReported:
						if (item.NotReported ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.Other:
						if (item.OtherDisability ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.RequiresWheelchair:
						if (item.WheelChair ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.SpecialDiet:
						if (item.SpecialDiet ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.Unknown:
						if (item.UnknownSpecialNeeds ?? false)
							itemHasThisDisability = true;
						break;
					case (int)DisabilityEnum.VisualImpairment:
						if (item.VisualProblem ?? false)
							itemHasThisDisability = true;
						break;
				}
				if (itemHasThisDisability)
					foreach (var header in Headers)
						if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders)
								if (item.ClientTypeID == (int)subheader.Code || subheader.Code == ReportTableSubHeaderEnum.Total)
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
			}

			if (UseNonDuplicatedSubtotal)
				foreach (var header in Headers)
					if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
						foreach (var subheader in header.SubHeaders)
							if (item.ClientTypeID == (int)subheader.Code || subheader.Code == ReportTableSubHeaderEnum.Total)
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
		}
	}
}