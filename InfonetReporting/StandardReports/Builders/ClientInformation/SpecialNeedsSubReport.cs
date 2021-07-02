using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.ClientInformation.SpecialNeeds;

namespace Infonet.Reporting.StandardReports.Builders.ClientInformation {
	public class ClientInformationSpecialNeedsSubReport : SubReportCountBuilder<ClientCase, ClientInformationSpecialNeedsLineItem> {
		public ClientInformationSpecialNeedsSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Client Type", "Primary Language", "ADL Problem", "Deaf", "Developmental Disability", "Immobile", "Limited English", "Meds Administered", "Mental Disability", "Special Diet", "Visual Problem", "Wheel Chair", "Other Disability", "No Special Needs", "Unkown", "Not Reported" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ClientInformationSpecialNeedsLineItem record) {
			csv.WriteField(record.ClientID);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.ClientType[record.ClientTypeID]?.Description);
			csv.WriteField(Lookups.Language[record.PrimaryLanguageID]?.Description);
			csv.WriteField(record.ADLProblem ?? false ? "X" : "");
			csv.WriteField(record.Deaf ?? false ? "X" : "");
			csv.WriteField(record.DevelopmentalDisabled ?? false ? "X" : "");
			csv.WriteField(record.Immobile ?? false ? "X" : "");
			csv.WriteField(record.LimitedEnglish ?? false ? "X" : "");
			csv.WriteField(record.MedsAdministered ?? false ? "X" : "");
			csv.WriteField(record.MentalDisability ?? false ? "X" : "");
			csv.WriteField(record.SpecialDiet ?? false ? "X" : "");
			csv.WriteField(record.VisualProblem ?? false ? "X" : "");
			csv.WriteField(record.WheelChair ?? false ? "X" : "");
			csv.WriteField(record.OtherDisability ?? false ? "X" : "");
			csv.WriteField(record.NoSpecialNeeds ?? false ? "X" : "");
			csv.WriteField(record.UnknownSpecialNeeds ?? false ? "X" : "");
			csv.WriteField(record.NotReported ?? false ? "X" : "");
		}

		protected override IEnumerable<ClientInformationSpecialNeedsLineItem> PerformSelect(IQueryable<ClientCase> query) {
			return query.Select(m => new ClientInformationSpecialNeedsLineItem {
				ClientID = m.ClientId,
				ClientCode = m.Client.ClientCode,
				CaseID = m.CaseId,
				ClientStatus = m.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && m.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				ClientTypeID = m.Client.ClientTypeId,
				DevelopmentalDisabled = m.ClientDisability.DevelopmentallyDisabled,
				SpecialDiet = m.ClientDisability.SpecialDiet,
				Immobile = m.ClientDisability.Immobil,
				WheelChair = m.ClientDisability.WheelChair,
				MedsAdministered = m.ClientDisability.MedsAdministered,
				Deaf = m.ClientDisability.Deaf,
				VisualProblem = m.ClientDisability.VisualProblem,
				LimitedEnglish = m.ClientDisability.LimitedEnglish,
				PrimaryLanguageID = m.ClientDisability.PrimaryLanguageID,
				ADLProblem = m.ClientDisability.ADLProblem,
				OtherDisability = m.ClientDisability.OtherDisability,
				MentalDisability = m.ClientDisability.MentalDisability,
				NoSpecialNeeds = m.ClientDisability.NoSpecialNeeds,
				UnknownSpecialNeeds = m.ClientDisability.UnknownSpecialNeeds,
				NotReported = m.ClientDisability.NotReported
			});
		}

		protected override void CreateReportTables() {
			//All
			var newAndOnGoingAllClientTypes = GetNewAndOngoingHeaders();

			if (ReportContainer.Provider == Provider.DV) {
				// Special Needs
				var specialNeedsGroup = new ClientSpecialNeedsReportTable("Special Needs", 1);
				specialNeedsGroup.Headers = newAndOnGoingAllClientTypes;
				specialNeedsGroup.UseNonDuplicatedSubtotal = true;
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Assistance w/ ADL", Code = 1, Order = 1 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Developmental Disability", Code = 2, Order = 3 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Hearing Impairment", Code = 3, Order = 2 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Immobility", Code = 4, Order = 4 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Limited English", Code = 5, Order = 5 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Medication Administered", Code = 6, Order = 6 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Requires Wheelchair Accessibility", Code = 8, Order = 8 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Special Diet", Code = 9, Order = 9 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Visual Impairment", Code = 10, Order = 10 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "No Special Needs Indicated", Code = 11, Order = 11 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Not Reported", Code = 12, Order = 12 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Other", Code = 13, Order = 7 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Unknown", Code = 14, Order = 13 });
				ReportTableList.Add(specialNeedsGroup);

				// Disabling Condition
				var disablingConditionGroup = new DisablingConditionReportTable("Disabling Condition", 2);
				disablingConditionGroup.Headers = newAndOnGoingAllClientTypes;
				disablingConditionGroup.Rows.Add(new ReportRow { Title = "Yes", Code = 1, Order = 1 });
				disablingConditionGroup.Rows.Add(new ReportRow { Title = "No", Code = 2, Order = 2 });
				disablingConditionGroup.Rows.Add(new ReportRow { Title = "Not Reported", Code = 3, Order = 3 });
				disablingConditionGroup.Rows.Add(new ReportRow { Title = "Unknown", Code = 4, Order = 4 });
				ReportTableList.Add(disablingConditionGroup);

				// Disabling Condition
				var physicalDisabilityGroup = new PhysicalDisabilityReportTable("Physical Disability", 3);
				physicalDisabilityGroup.Headers = newAndOnGoingAllClientTypes;
				physicalDisabilityGroup.Rows.Add(new ReportRow { Title = "Yes", Code = 1, Order = 1 });
				physicalDisabilityGroup.Rows.Add(new ReportRow { Title = "No", Code = 2, Order = 2 });
				ReportTableList.Add(physicalDisabilityGroup);

				// Developmental Disability
				var developmentalDisabilityGroup = new DevelopmentalDisabilityReportTable("Developmental Disability", 4);
				developmentalDisabilityGroup.Headers = newAndOnGoingAllClientTypes;
				developmentalDisabilityGroup.Rows.Add(new ReportRow { Title = "Yes", Code = 1, Order = 1 });
				developmentalDisabilityGroup.Rows.Add(new ReportRow { Title = "No", Code = 2, Order = 2 });
				ReportTableList.Add(developmentalDisabilityGroup);
			} else if (ReportContainer.Provider == Provider.SA) {
				var specialNeedsGroup = new ClientSpecialNeedsReportTable("Special Needs", 1);
				specialNeedsGroup.Headers = newAndOnGoingAllClientTypes;
				specialNeedsGroup.UseNonDuplicatedSubtotal = true;
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Developmental Disability", Code = 2, Order = 2 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Hard of hearing/deaf", Code = 3, Order = 1 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Limited English", Code = 5, Order = 3 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Low vision/blind", Code = 10, Order = 7 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Mental/Emotional Disability", Code = 7, Order = 4 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Requires Wheelchair Accessibility", Code = 8, Order = 6 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Other Physical Disability", Code = 13, Order = 5 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "None Identified", Code = 11, Order = 8 });
				ReportTableList.Add(specialNeedsGroup);
			} else if (ReportContainer.Provider == Provider.CAC) {
				var specialNeedsGroup = new ClientSpecialNeedsReportTable("Special Needs", 1);
				specialNeedsGroup.Headers = newAndOnGoingAllClientTypes;
				specialNeedsGroup.HideSubtotal = true;
				specialNeedsGroup.UseNonDuplicatedSubtotal = true;
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Developmental Disability", Code = 2, Order = 4 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Hearing Impairment", Code = 3, Order = 1 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Limited English Proficiency, Requires Interpreter", Code = 5, Order = 7 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Mental Health Disability", Code = 7, Order = 5 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Visual Impairment", Code = 10, Order = 2 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Requires Wheelchair Accessibility", Code = 8, Order = 3 });
				specialNeedsGroup.Rows.Add(new ReportRow { Title = "Other Physical Disability", Code = 13, Order = 6 });
				ReportTableList.Add(specialNeedsGroup);
			}

			// Primary Languages
			var primaryLanguagesGroup = new PrimaryLanguagesReportTable("Primary Languages", 5);
			primaryLanguagesGroup.Headers = newAndOnGoingAllClientTypes;
			ReportTableList.Add(primaryLanguagesGroup);
		}
	}

	public class ClientInformationSpecialNeedsLineItem {
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? CaseID { get; set; }
		public bool? DevelopmentalDisabled { get; set; }
		public bool? SpecialDiet { get; set; }
		public bool? Immobile { get; set; }
		public bool? WheelChair { get; set; }
		public bool? MedsAdministered { get; set; }
		public bool? Deaf { get; set; }
		public bool? VisualProblem { get; set; }
		public bool? LimitedEnglish { get; set; }
		public int? PrimaryLanguageID { get; set; }
		public bool? ADLProblem { get; set; }
		public bool? OtherDisability { get; set; }
		public bool? MentalDisability { get; set; }
		public bool? NoSpecialNeeds { get; set; }
		public bool? UnknownSpecialNeeds { get; set; }
		public bool? NotReported { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? ClientTypeID { get; set; }
	}
}