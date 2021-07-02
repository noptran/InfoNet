using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Services;

namespace Infonet.Reporting.StandardReports.ReportTables.Services.CommunityGroup {
	public class CommunityGroupServiceReportTable : ReportTable<CommunityGroupSubReportLineItem> {
		private readonly Dictionary<int?, HashSet<int>> _uniqueStaffLists = new Dictionary<int?, HashSet<int>>();
		private readonly Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>> _uniqueStaffByType = new Dictionary<ReportTableHeaderEnum, Dictionary<ReportTableSubHeaderEnum, HashSet<int>>>();
		private ISet<int?> _fundingSourceIds = null;

		public CommunityGroupServiceReportTable(string title, int displayOrder) : base(title, displayOrder) {
			RowPredicate = (r, i) => r.Code == i.ServiceId;
		}

		public Func<ReportRow, CommunityGroupSubReportLineItem, bool> RowPredicate { get; set; }

		public IEnumerable<int?> FundingSourceIds {
			get { return _fundingSourceIds; }
			set { _fundingSourceIds = value.NotNull(v => new HashSet<int?>(v)); }
		}

		public override void PreCheckAndApply(ReportContainer container) {
			foreach (var header in Headers) {
				var innerDict = new Dictionary<ReportTableSubHeaderEnum, HashSet<int>>();
				foreach (var subheader in header.SubHeaders)
					innerDict.Add(subheader.Code, new HashSet<int>());
				_uniqueStaffByType.Add(header.Code, innerDict);
			}
		}

		public override void CheckAndApply(CommunityGroupSubReportLineItem item) {
			var svIds = item.Staff.Select(s => s.SvId).ToArray();
			foreach (var row in Rows.Where(r => RowPredicate.Invoke(r, item))) {
				CheckStaffLists(row.Code, svIds);
				foreach (var header in Headers) {
					foreach (var subheader in header.SubHeaders)
						switch (header.Code) {
							case ReportTableHeaderEnum.NumberOfPresentations:
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.NumberOfPresentations;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.NumberOfPresentations;
								break;
							case ReportTableHeaderEnum.NumberOfParticipants:
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.NumberOfParticipants;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.NumberOfParticipants;
								break;
							case ReportTableHeaderEnum.NumberOfStaff:
								foreach (var eachSet in _uniqueStaffLists.Values)
									_uniqueStaffByType[header.Code][subheader.Code].AddRange(eachSet);
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] = _uniqueStaffLists[row.Code].Count;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] = _uniqueStaffByType[header.Code][subheader.Code].Count;
								break;
							case ReportTableHeaderEnum.PresentationHours:
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.PresentationHours ?? 0.0D;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += item.PresentationHours ?? 0.0D;
								break;
							case ReportTableHeaderEnum.StaffConductHours:
								double conductHours = _fundingSourceIds == null
									? item.Staff.Sum(s => s.ConductHours)
									: item.Staff.Sum(s => s.ConductHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0));
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += conductHours;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += conductHours;
								break;
							case ReportTableHeaderEnum.StaffTravelHours:
								double travelHours = _fundingSourceIds == null
									? item.Staff.Sum(s => s.TravelHours)
									: item.Staff.Sum(s => s.TravelHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0));
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += travelHours;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += travelHours;
								break;
							case ReportTableHeaderEnum.StaffPreparationHours:
								double prepHours = _fundingSourceIds == null
									? item.Staff.Sum(s => s.PrepHours)
									: item.Staff.Sum(s => s.PrepHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0));
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += prepHours;
								NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += prepHours;
								break;
						}
				}
			}
		}

		private void CheckStaffLists(int? code, IEnumerable<int> svids) {
			HashSet<int> svidList;
			if (_uniqueStaffLists.TryGetValue(code, out svidList))
				svidList.AddRange(svids);
			else
				_uniqueStaffLists.Add(code, new HashSet<int>(svids));
		}
	}

	#region DV Enums
	public enum DVInformationReferralServiceEnum {
		[Display(Name = "Judge")] Judge = 8,
		[Display(Name = "Law Enforcement")] LawEnforcement = 9,

		[Display(Name = "Medical and Hospital")]
		MedicalAndHospital = 10,
		[Display(Name = "Other")] Other = 11,
		[Display(Name = "Social Service")] SocialService = 12,
		[Display(Name = "State's Attorney")] StatesAttorney = 13,
		[Display(Name = "Teacher/Educator")] TeacherEducator = 14
	}

	public enum DVProfessionalTrainingServiceEnum {
		[Display(Name = "Clergy")] Clergy = 30,
		[Display(Name = "Judge")] Judge = 32,
		[Display(Name = "Law Enforcement/CJ")] LawEnforcement = 33,

		[Display(Name = "Medical and Hospital")]
		MedicalHospital = 34,
		[Display(Name = "Other")] Other = 37,
		[Display(Name = "Social Service")] SocialService = 38,
		[Display(Name = "State's Attorney")] StatesAttorney = 39,
		[Display(Name = "Teacher/Educator")] TeacherEducator = 40
	}

	public enum DVPublicEducationServiceEnum {
		[Display(Name = "Civic Organization")] CivicOrganization = 17,

		[Display(Name = "Employees/Employers")]
		EmployeesEmployers = 18,
		[Display(Name = "Other")] Other = 19,

		[Display(Name = "Other Client Groups")]
		OtherClientGroups = 20,

		[Display(Name = "Religious Organization")]
		ReligiousOrganization = 21,
		[Display(Name = "Youth Organization")] YouthOrganization = 22
	}

	public enum DVSchoolServiceEnum {
		[Display(Name = "Primary", Order = 1)] Primary = 29,

		[Display(Name = "Preschool", Order = 2)]
		Preschool = 28,

		[Display(Name = "Post Secondary", Order = 3)]
		PostSecondary = 27,

		[Display(Name = "Kindergarten", Order = 4)]
		Kindergarten = 26,

		[Display(Name = "Junior High", Order = 5)]
		JuniorHigh = 25,

		[Display(Name = "High School", Order = 6)]
		HighSchool = 24
	}

	public enum DVTrainingServiceEnum {
		[Display(Name = "In-service Staff")] InServiceStaff = 74,

		[Display(Name = "In-service Volunteer")]
		InServiceVolunteer = 31,
		[Display(Name = "New Staff")] NewStaff = 35,
		[Display(Name = "New Volunteer")] NewVolunteer = 36
	}

	public enum OtherActivitiesServiceEnum {
		[Display(Name = "Board Activities")] BoardActivities = 2,

		[Display(Name = "Non-direct Service Volunteer Activity")]
		NonDirectServiceVolunteerActivity = 15
	}
	#endregion

	#region SA Enums
	public enum SAPublicEducationServiceEnum {
		[Display(Name = "Civic Organization", Order = 1)]
		CivicOrganization = 17,

		[Display(Name = "Elementary (age 5-10)", Order = 2)]
		Elementary = 80,

		[Display(Name = "Employees/Employers", Order = 3)]
		EmployeesEmployers = 18,

		[Display(Name = "Junior High/Middle School (age 11-13)", Order = 4)]
		JuniorMiddleSchool = 81,

		[Display(Name = "Other Client Groups", Order = 5)]
		OtherClientGroups = 20,

		[Display(Name = "People with Disabilities", Order = 6)]
		PeopleWithDisablities = 124,

		[Display(Name = "Post Secondary (age 20-24)", Order = 7)]
		PostSecondary2024 = 83,

		[Display(Name = "Post Secondary (age 18-19)", Order = 8)]
		PostSecondary1819 = 82,

		[Display(Name = "Pre-school (up to age 5)", Order = 9)]
		Preschool = 84,

		[Display(Name = "Religious Organization", Order = 10)]
		ReligiousOrganization = 21,

		[Display(Name = "Secondary (age 14-18)", Order = 11)]
		Secondary = 85
	}

    public enum SACACProfessionalTrainingServiceEnum
    {
        [Display(Name = "Clergy")]
        Clergy = 30,
        [Display(Name = "Judge")]
        Judge = 32,
        [Display(Name = "Law Enforcement")]
        LawEnforcement = 86,

        [Display(Name = "Medical and Hospital")]
        MedicalHospital = 34,
        [Display(Name = "Other")]
        Other = 37,
        [Display(Name = "Social Service")]
        SocialService = 38,
        [Display(Name = "State's Attorney")]
        StatesAttorney = 39,
        [Display(Name = "Teacher/Educator")]
        TeacherEducator = 40
    }

    public enum SACACInformationReferralServiceEnum {
		[Display(Name = "Information and Referral")]
		InformationReferral = 6
	}

	public enum SACACInstitutionalAdvocacyEnum {
		[Display(Name = "Judge", Order = 1)] Judge = 8,

		[Display(Name = "Law Enforcement", Order = 2)]
		LawEnforcement = 9,

		[Display(Name = "Medical and Hospital", Order = 3)]
		MedicalAndHospital = 10,
		[Display(Name = "Other", Order = 4)] Other = 11,
		[Display(Name = "School", Order = 5)] School = 78,

		[Display(Name = "Social Service", Order = 6)]
		SocialService = 12,

		[Display(Name = "State's Attorney", Order = 7)]
		StatesAttorney = 13
	}

	public enum SACACTrainingServiceEnum {
		[Display(Name = "In-service Volunteer")]
		InServiceVolunteer = 31,
		[Display(Name = "New Volunteer")] NewVolunteer = 36
	}
	#endregion

	#region CAC Enums
	public enum CACPublicEducationServiceEnum {
		[Display(Name = "Civic Organization", Order = 1)]
		CivicOrganization = 17,

		[Display(Name = "Elementary (age 5-10)", Order = 2)]
		Elementary = 80,

		[Display(Name = "Employees/Employers", Order = 3)]
		EmployeesEmployers = 18,

		[Display(Name = "Junior High/Middle School (age 11-13)", Order = 4)]
		JuniorMiddleSchool = 81,

		[Display(Name = "Other Client Groups", Order = 5)]
		OtherClientGroups = 20,

		[Display(Name = "Post Secondary (age 20-24)", Order = 6)]
		PostSecondary2024 = 83,

		[Display(Name = "Post Secondary (age 18-19)", Order = 7)]
		PostSecondary1819 = 82,

		[Display(Name = "Pre-school (up to age 5)", Order = 8)]
		Preschool = 84,

		[Display(Name = "Religious Organization", Order = 9)]
		ReligiousOrganization = 21,

		[Display(Name = "Secondary (age 14-18)", Order = 10)]
		Secondary = 85
	}
	#endregion
}