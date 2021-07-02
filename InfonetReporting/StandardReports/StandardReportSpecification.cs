using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.StandardReports {
	public class StandardReportSpecification {
		[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
		public StandardReportSpecification() { }

		public StandardReportSpecification(StandardReportSpecification specification) {
			Title = specification.Title;
			Provider = specification.Provider;
			StartDate = specification.StartDate;
			EndDate = specification.EndDate;
			CenterIds = specification.CenterIds?.ToArray();
			ReportSelection = specification.ReportSelection;
			SubReportSelections = specification.SubReportSelections?.ToArray();
			SvIds = specification.SvIds?.ToArray();
			ServiceIds = specification.ServiceIds?.ToArray();
			OffenderRelationshipIds = specification.OffenderRelationshipIds?.ToArray();
			FundingSourceIds = specification.FundingSourceIds?.ToArray();
			GenderIds = specification.GenderIds?.ToArray();
			EthnicityIds = specification.EthnicityIds?.ToArray();
			RaceIds = specification.RaceIds?.ToArray();
			CityOrTowns = specification.CityOrTowns?.ToArray();
			Townships = specification.Townships?.ToArray();
			CountyIds = specification.CountyIds?.ToArray();
			Zipcodes = specification.Zipcodes?.ToArray();
			StateIds = specification.StateIds?.ToArray();
			ClientTypeIds = specification.ClientTypeIds?.ToArray();
			MinimumAge = specification.MinimumAge;
			MaximumAge = specification.MaximumAge;
			SpecialCenterSelectionType = specification.SpecialCenterSelectionType;
			SpecialFundingSelectionType = specification.SpecialFundingSelectionType;
		}

		[DisplayName("Report Title")]
		public string Title { get; set; }

		public Provider Provider { get; set; }

		[DisplayName("Begin Date")]
		public DateTime? StartDate { get; set; }

		[DisplayName("End Date")]
		public DateTime? EndDate { get; set; }

		public int?[] CenterIds { get; set; }

		public ReportSelection ReportSelection { get; set; }

		[Required]
		public SubReportSelection[] SubReportSelections { get; set; }

		[Display(Name = "Staff Name")]
		public int?[] SvIds { get; set; }

		[Display(Name = "Service Name")]
		public int?[] ServiceIds { get; set; }

		[Display(Name = "Offender Relationship to Victim")]
		public int?[] OffenderRelationshipIds { get; set; }

		[Display(Name = "Funding")]
		public int?[] FundingSourceIds { get; set; }

		[Display(Name = "Gender")]
		public int?[] GenderIds { get; set; }

		[Display(Name = "Ethnicity")]
		public int?[] EthnicityIds { get; set; }

		[Display(Name = "Race")]
		public int?[] RaceIds { get; set; }

		[Display(Name = "City or Town")]
		public string[] CityOrTowns { get; set; }

		[Display(Name = "Township")]
		public string[] Townships { get; set; }

		[Display(Name = "County")]
		public int?[] CountyIds { get; set; }

		[Display(Name = "Zip Code")]
		public string[] Zipcodes { get; set; }

		[Display(Name = "State")]
		public int?[] StateIds { get; set; }

		[Display(Name = "Client Type")]
		public int?[] ClientTypeIds { get; set; }

		[Display(Name = "Age Range")]
		public int? MinimumAge { get; set; }

		public int? MaximumAge { get; set; }

		[DisplayName("Center(s)")]
		public string SpecialCenterSelectionType { get; set; }

		[DisplayName("Funding Source")]
		public string SpecialFundingSelectionType { get; set; }
	}
}