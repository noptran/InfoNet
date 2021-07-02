using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics;

namespace Infonet.Reporting.StandardReports.Builders.ClientInformation {
	public class ClientInformationDemographicsSubReport : SubReportCountBuilder<ClientCase, ClientInformationDemographicsLineItem> {
		private readonly int[] _shelterTypeValues = { (int)ShelterServiceEnum.OnsiteShelter, (int)ShelterServiceEnum.OffsiteShelter, (int)ShelterServiceEnum.TransitionalHousing };

		public ClientInformationDemographicsSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

        public Dictionary<RaceHudCompositeEnum, int> RaceHudCompositeOrder = new Dictionary<RaceHudCompositeEnum, int> {
            {RaceHudCompositeEnum.AmericanIndianOrAlaskaNativeAndBlackOrAfricanAmerican, 200 },
            {RaceHudCompositeEnum.AmericanIndianOrAlaskaNativeAndWhite,210 },
            {RaceHudCompositeEnum.AsianAndWhite, 220 },
            {RaceHudCompositeEnum.AsianORSouthAsian,14 },
            {RaceHudCompositeEnum.BlackOrAfricanAmericanAndWhite,240 },
            {RaceHudCompositeEnum.MENAORWhite,63 },
            {RaceHudCompositeEnum.OtherMultiracial,260 }
        };

        protected override IEnumerable<ClientInformationDemographicsLineItem> PerformSelect(IQueryable<ClientCase> query) {
			return query.Select(m => new ClientInformationDemographicsLineItem {
				ClientCode = m.Client.ClientCode,
				CaseID = m.CaseId,
				ClientID = m.ClientId,
				ClientStatus = m.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && m.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				ClientTypeID = m.Client.ClientTypeId,
				FirstContactDate = m.FirstContactDate,
				HouseholdID = m.InvestigationClients.Any() ? m.InvestigationClients.Select(i => i.T_CACInvestigations_FK).FirstOrDefault().Value.ToString() : m.ClientId + ":" + m.CaseId,
				ClientShelterTypeIDs = m.ServiceDetailsOfClient.Where(sd => sd.ShelterBegDate.HasValue && sd.ShelterBegDate <= ReportContainer.EndDate && (sd.ShelterEndDate == null || sd.ShelterEndDate >= ReportContainer.StartDate) && _shelterTypeValues.Contains(sd.ServiceID)).Select(ct => ct.ServiceID),
				GenderIdentityID = m.Client.GenderIdentityId,
				SexualOrientationID = m.SexualOrientationId,
				EthnicityID = m.Client.EthnicityId,
				RaceIDs = m.Client.ClientRaces.Select(r => r.RaceHudId),
				RaceId = m.Client.RaceId,
				AgeAtFirstContact = m.Age,
				VetStatusID = m.VetStatusId,
				EmploymentID = m.EmploymentId,
				EducationID = m.EducationId,
				SchoolID = m.SchoolId,
				HealthInsuranceID = m.HealthInsuranceId,
				NCB_FoodBenefit = m.ClientNonCashBenefits.FoodBenefit,
				NCB_OtherSource = m.ClientNonCashBenefits.OtherSource,
				NCB_OtherTANF = m.ClientNonCashBenefits.TANFOther,
				NCB_PublicHousing = m.ClientNonCashBenefits.PublicHousing,
				NCB_SpecSuppNutr = m.ClientNonCashBenefits.SpecSuppNutr,
				NCB_TANFChildCare = m.ClientNonCashBenefits.TANFChildCare,
				NCB_TANFTrans = m.ClientNonCashBenefits.TANFTrans,
				NCB_NoBenefit = m.ClientNonCashBenefits.NoBenefit,
				NCB_Unknown = m.ClientNonCashBenefits.UnknownBenefit,
				Ins_Medicaid = m.ClientNonCashBenefits.Medicaid,
				Ins_Medicare = m.ClientNonCashBenefits.Medicare,
				Ins_NoHealthIns = m.ClientNonCashBenefits.NoHealthIns,
				Ins_Private = m.ClientNonCashBenefits.PrivateIns,
				Ins_StateChildHealth = m.ClientNonCashBenefits.StateChildHealth,
				Ins_Unknown = m.ClientNonCashBenefits.UnknownHealthIns,
				Ins_VetAdminMed = m.ClientNonCashBenefits.VetAdminMed,
				MaritalStatusID = m.MaritalStatusId,
				PregnantID = m.PregnantId,
				IncomeSources = m.FinancialResources.Select(f => new IncomeSource { IncomeID = f.IncomeSource2ID, Amount = f.Amount }),
				PrimaryIncomeSources = m.FinancialResources.Where(f => m.FinancialResources.GroupBy(cf => cf.ClientID).Select(cf => cf.Max(a => a.Amount)).FirstOrDefault() == f.Amount).Select(f => new IncomeSource { IncomeID = f.IncomeSource2ID, Amount = f.Amount }),
				MonthlyIncome = m.FinancialResources.Where(a => -1 != a.Amount).Sum(a => a.Amount),
				SAPrimaryIncomeSource = m.ClientIncome.PrimaryIncomeId,
				NumberOfChildren = m.NumberOfChildren,
				DCFSInvestigation = m.DCFSInvestigation,
				DCFSOpen = m.DCFSOpen,
				CustodyID = m.CustodyId,
				LivesWithID = m.LivesWithId,
				IsCollegeStudent = m.CollegeUnivStudentId,
				RelationshipToVictimID = m.RelationSOtoClientId
			});
		}

		protected override string[] CsvHeaders {
			get {
				if (ReportContainer.Provider == Provider.DV)
					return new[] { "ID", "Client ID", "Case ID", "Client Type", "Client Status", "First Contact Date", "Client Shelter Type", "Gender", "Ethnicity", "Race", "Sexual Orientation", "Age", "Veteran Status", "Employment", "Education", "School", "Medicaid Health Insurance", "Medicare Health Insurance", "State Childrens Health Insurance", "Veterans Administration Med Services", "Private Health Insurance", "No Health Insurance", "Unknown Health Insurance", "Pregnant", "NCB Food Benefit", "NCB Other Source", "NCB Other TANF", "NCB Public Housing", "NCB Special Supplemental Nutrition", "NCB TANF Child Care", "NCB TANF Transportation", "Primary Income Sources", "Monthly Income", "Number of Children", "DCFS Investigation", "DCFS Open", "Custody", "Lives With", "Marital Status" };

				return new[] { "ID", "Client ID", "Case ID", "Client Type", "Client Status", "First Contact Date", "Gender", "Ethnicity", "Race", "Sexual Orientation", "Age", "Employment", "Education", "College Student", "Health Insurance", "Pregnant", "Primary Income", "Number of Children", "Custody", "Relationship to Victim", "Marital Status" };
			}
		}

		protected override void WriteCsvRecord(CsvWriter csv, ClientInformationDemographicsLineItem record) {
			csv.WriteField(record.ClientID);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(Lookups.ClientType[record.ClientTypeID]?.Description);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(record.FirstContactDate, "M/d/yyyy");
			if (ReportContainer.Provider == Provider.DV)
				csv.WriteField(string.Join("|", record.ClientShelterTypeIDs.Select(ct => Lookups.ProgramsAndServices[ct].Description).DefaultIfEmpty("Walk-in")));
			csv.WriteField(Lookups.GenderIdentity[record.GenderIdentityID]?.Description);
			csv.WriteField(Lookups.Ethnicity[record.EthnicityID]?.Description);
			if (ReportContainer.Provider == Provider.CAC)
				csv.WriteField(Lookups.Race[record.RaceId]?.Description);
			else
				csv.WriteField(string.Join("|", record.RaceIDs.Select(r => Lookups.RaceHud[r].Description).DefaultIfEmpty("N/A")));
			csv.WriteField(Lookups.SexualOrientation[record.SexualOrientationID]?.Description);
			csv.WriteField(record.AgeAtFirstContact);
			if (ReportContainer.Provider == Provider.DV)
				csv.WriteField(Lookups.YesNo[record.VetStatusID]?.Description);
			csv.WriteField(Lookups.EmploymentType[record.EmploymentID]?.Description);
			csv.WriteField(Lookups.Education[record.EducationID]?.Description);
			if (ReportContainer.Provider == Provider.DV)
				csv.WriteField(Lookups.School[record.SchoolID]?.Description);
			else
				csv.WriteField(record.IsCollegeStudent.NotNull(i => i == 1 ? "Yes" : "No"));
			if (ReportContainer.Provider == Provider.DV) {
				csv.WriteField(record.Ins_Medicaid);
				csv.WriteField(record.Ins_Medicare);
				csv.WriteField(record.Ins_StateChildHealth);
				csv.WriteField(record.Ins_VetAdminMed);
				csv.WriteField(record.Ins_Private);
				csv.WriteField(record.Ins_NoHealthIns);
				csv.WriteField(record.Ins_Unknown);
			} else {
				csv.WriteField(Lookups.HealthInsurance[record.HealthInsuranceID]?.Description);
			}
			csv.WriteField(Lookups.Pregnant[record.PregnantID]?.Description);
			if (ReportContainer.Provider == Provider.DV) {
				csv.WriteField(record.NCB_FoodBenefit);
				csv.WriteField(record.NCB_OtherSource);
				csv.WriteField(record.NCB_OtherTANF);
				csv.WriteField(record.NCB_PublicHousing);
				csv.WriteField(record.NCB_SpecSuppNutr);
				csv.WriteField(record.NCB_TANFChildCare);
				csv.WriteField(record.NCB_TANFTrans);
				csv.WriteField(string.Join("|", record.PrimaryIncomeSources.Select(i => Lookups.IncomeSource2[i.IncomeID]?.Description).DefaultIfEmpty("Unassigned")));
				csv.WriteField(record.MonthlyIncome);
			} else {
				csv.WriteField(Lookups.IncomeSource[record.SAPrimaryIncomeSource]?.Description);
			}
			csv.WriteField(record.NumberOfChildren);
			if (ReportContainer.Provider == Provider.DV) {
				csv.WriteField(record.DCFSInvestigation.NotNull(i => i == 1 ? "Yes" : "No"));
				csv.WriteField(record.DCFSOpen.NotNull(i => i == 1 ? "Yes" : "No"));
			}
			csv.WriteField(Lookups.ChildCustody[record.CustodyID]?.Description);
			if (ReportContainer.Provider == Provider.DV)
				csv.WriteField(Lookups.ChildLivesWith[record.LivesWithID]?.Description);
			else
				csv.WriteField(Lookups.RelationshipToClient[record.RelationshipToVictimID]?.Description);
			csv.WriteField(Lookups.MaritalStatus[record.MaritalStatusID]?.Description);
		}

		protected override void CreateReportTables() {
			//All
			var newAndOnGoingAllClientTypes = GetNewAndOngoingHeaders();
			//DV
			var newAndOnGoingAdult = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Adult, false);
			var newAndOnGoingChild = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Child, false);
			//CAC
			var newAndOnGoingChildVictim = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false);
			var newAndOnGoingCareTaker = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.NonOffendingCaretaker, false);

			/////////////////// Shared Between Systems //////////////////
			// Households
			var householdGroup = new HouseholdsReportTable(ReportContainer.Provider == Provider.CAC ? "Total Investigations" : "Total Households", 1) {
				HideHeaders = true,
				HideSubheaders = true,
				HideTitle = true,
				HideZeroValues = true,
				HideSubtotal = true
			};
			var householdHeaders = new List<ReportTableHeader>();
			var headerTotal = new ReportTableHeader("Total", ReportTableHeaderEnum.Total, new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = "Total" } });
			householdHeaders.Add(headerTotal);
			householdGroup.Headers = householdHeaders;
			householdGroup.Rows.Add(new ReportRow { Title = "Total Households" });
			ReportTableList.Add(householdGroup);

			var clientCaseCountsGroup = new ReportTableGroup<ClientInformationDemographicsLineItem>("", 2);
			clientCaseCountsGroup.Headers = newAndOnGoingAllClientTypes;
			clientCaseCountsGroup.HideTitle = true;
			clientCaseCountsGroup.HideSubtotal = true;
			// Cases
			if (ReportContainer.Provider != Provider.SA) {
				var caseCountGroup = new CaseCountReportTable("Client Cases", 2) {
					HideTitle = true,
					HideSubtotal = true,
					Headers = newAndOnGoingAllClientTypes
				};
				caseCountGroup.Rows.Add(new ReportRow { Title = "Client Cases" });
				clientCaseCountsGroup.ReportTables.Add(caseCountGroup);
			}

			// Clients
			var clientCountGroup = new ClientCountReportTable("Total Clients", 3);
			clientCountGroup.Headers = newAndOnGoingAllClientTypes;
			if (ReportContainer.Provider != Provider.SA) {
				clientCountGroup.HideHeaders = true;
				clientCountGroup.HideTitle = true;
			}
			clientCountGroup.HideSubtotal = true;
			clientCountGroup.Rows.Add(new ReportRow { Title = "Total Clients" });
			clientCaseCountsGroup.ReportTables.Add(clientCountGroup);

			ReportTableList.Add(clientCaseCountsGroup);

			// Shelter Types
			if (ReportContainer.Provider == Provider.DV) {
				var clientShelterTypeGroup = new ClientTypeReportTable("Client Type", 4) {
					Headers = GetNewAndOngoingHeaders(),
					HideSubtotal = true
				};
				double shelterOrder = 0;
				foreach (ShelterServiceEnum shelterType in Enum.GetValues(typeof(ShelterServiceEnum))) {
					switch (shelterType) {
						case ShelterServiceEnum.Walkin:
							shelterOrder = 1;
							break;
						case ShelterServiceEnum.OnsiteShelter:
							shelterOrder = 2;
							break;
						case ShelterServiceEnum.OffsiteShelter:
							shelterOrder = 3;
							break;
						case ShelterServiceEnum.TransitionalHousing:
							shelterOrder = 4;
							break;
					}
					clientShelterTypeGroup.Rows.Add(new ReportRow { Title = shelterType.GetDisplayName(), Code = (int)shelterType, Order = shelterOrder });
				}
				ReportTableList.Add(clientShelterTypeGroup);
			}

			// Gender
			var genderGroup = new GenderReportTable("Gender Identity", 5);
			genderGroup.Headers = newAndOnGoingAllClientTypes;
			genderGroup.PreHeader = "Basic Demographic Information (Adult and Child)";
			foreach (var item in Lookups.GenderIdentity[ReportContainer.Provider])
				genderGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(genderGroup);

			// Ethnicity
			if (ReportContainer.Provider != Provider.CAC) {
				var ethnicityGroup = new EthnicityReportTable("Ethnicity", 6);
				ethnicityGroup.Headers = newAndOnGoingAllClientTypes;
				foreach (var item in Lookups.Ethnicity[ReportContainer.Provider])
					ethnicityGroup.Rows.Add(GetReportRowFromLookup(item));
				ReportTableList.Add(ethnicityGroup);
			}

			// Race
			if (ReportContainer.Provider != Provider.CAC) {
				var raceGroup = new RaceHudReportTable("Race", 7);
				raceGroup.Headers = newAndOnGoingAllClientTypes;
				raceGroup.UseNonDuplicatedSubtotal = true;
                raceGroup.UseNonDuplicatedSubtotalLabel = true;
				foreach (var item in Lookups.RaceHud[ReportContainer.Provider])
					raceGroup.Rows.Add(GetReportRowFromLookup(item));
				foreach (RaceHudCompositeEnum item in Enum.GetValues(typeof(RaceHudCompositeEnum)))
					raceGroup.Rows.Add(new ReportRow { Title = item.GetDisplayName(), Code = (int)item, Order = RaceHudCompositeOrder[item] });
				ReportTableList.Add(raceGroup);
			} else {
				var raceGroup = new RaceReportTable("Race", 7);
				raceGroup.Headers = newAndOnGoingAllClientTypes;
				foreach (var item in Lookups.Race[ReportContainer.Provider])
					raceGroup.Rows.Add(GetReportRowFromLookup(item));
				ReportTableList.Add(raceGroup);
			}

			// Age
			var ageGroup = new AgeReportTable("Age At First Contact", 8);
			ageGroup.Headers = newAndOnGoingAllClientTypes;
			ageGroup.Rows.Add(new ReportRow { Title = "Unknown", Code = -1, Order = 1 });
			ageGroup.Rows.Add(new ReportRow { Title = "0-1", Code = 1, Order = 2 });
			ageGroup.Rows.Add(new ReportRow { Title = "2-3", Code = 2, Order = 3 });
			ageGroup.Rows.Add(new ReportRow { Title = "4-5", Code = 3, Order = 4 });
			ageGroup.Rows.Add(new ReportRow { Title = "6-7", Code = 4, Order = 5 });
			ageGroup.Rows.Add(new ReportRow { Title = "8-9", Code = 5, Order = 6 });
			ageGroup.Rows.Add(new ReportRow { Title = "10-11", Code = 6, Order = 7 });
			ageGroup.Rows.Add(new ReportRow { Title = "12-13", Code = 7, Order = 8 });
			ageGroup.Rows.Add(new ReportRow { Title = "14-15", Code = 8, Order = 9 });
			ageGroup.Rows.Add(new ReportRow { Title = "16-17", Code = 9, Order = 10 });
			ageGroup.Rows.Add(new ReportRow { Title = "18-19", Code = 10, Order = 11 });
			ageGroup.Rows.Add(new ReportRow { Title = "20-29", Code = 11, Order = 12 });
			ageGroup.Rows.Add(new ReportRow { Title = "30-39", Code = 12, Order = 13 });
			ageGroup.Rows.Add(new ReportRow { Title = "40-49", Code = 13, Order = 14 });
			ageGroup.Rows.Add(new ReportRow { Title = "50-59", Code = 14, Order = 15 });
			ageGroup.Rows.Add(new ReportRow { Title = "60-64", Code = 15, Order = 16 });
			ageGroup.Rows.Add(new ReportRow { Title = "65+", Code = 16, Order = 17 });
			ReportTableList.Add(ageGroup);

			if (ReportContainer.Provider == Provider.DV || ReportContainer.Provider == Provider.SA) {

				//////// Adult Client Information ///////
				// Sexual Orientation
				var sexualOrientationGroup = new SexualOrientationReportTable("Sexual Orientation", 9);
                if (ReportContainer.Provider == Provider.DV) {
                    sexualOrientationGroup.Headers = newAndOnGoingAdult;
                    sexualOrientationGroup.PreHeader = "Adult Client Information";
                    sexualOrientationGroup.HideSubheaders = true;
                }
                else {
                    sexualOrientationGroup.Headers = newAndOnGoingAllClientTypes;
                    sexualOrientationGroup.HideSubheaders = false;
                }               
				
				foreach (var item in Lookups.SexualOrientation[ReportContainer.Provider])
					sexualOrientationGroup.Rows.Add(GetReportRowFromLookup(item));
				sexualOrientationGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(sexualOrientationGroup);
			}

			////////////////// DV ONLY //////////////////
			if (ReportContainer.Provider == Provider.DV) {
				// Veteran's Status
				var veteransGroup = new VeteransStatusReportTable("Veteran's Status", 10);
				veteransGroup.Headers = newAndOnGoingAdult;
				veteransGroup.HideSubheaders = true;
				foreach (var item in Lookups.YesNo[ReportContainer.Provider])
					veteransGroup.Rows.Add(GetReportRowFromLookup(item));
				veteransGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(veteransGroup);

				// Employment
				var employmentGroup = new EmploymentReportTable("Employment", 11);
				employmentGroup.Headers = newAndOnGoingAdult;
				employmentGroup.HideSubheaders = true;

				foreach (var item in Lookups.EmploymentType[ReportContainer.Provider])
					employmentGroup.Rows.Add(GetReportRowFromLookup(item));
				employmentGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(employmentGroup);

				// Education
				var educationGroup = new AdultEducationReportTable("Education", 12);
				educationGroup.Headers = newAndOnGoingAdult;
				educationGroup.HideSubheaders = true;
				foreach (var item in Lookups.Education[ReportContainer.Provider])
					educationGroup.Rows.Add(GetReportRowFromLookup(item));
				educationGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(educationGroup);

				// Health insurance
				var healthInsuranceGroup = new HealthInsuranceDVReportTable("Health Insurance", 13);
				healthInsuranceGroup.Headers = newAndOnGoingAdult;
				healthInsuranceGroup.HideSubheaders = true;
				healthInsuranceGroup.UseNonDuplicatedSubtotal = true;
				foreach (var item in Lookups.HealthInsurance2[ReportContainer.Provider])
					healthInsuranceGroup.Rows.Add(GetReportRowFromLookup(item));
				healthInsuranceGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(healthInsuranceGroup);

				// Non Cash Benefits
				var nonCashBenefitGroup = new NonCashBenefitsReportTable("Non Cash Benefit", 14);
				nonCashBenefitGroup.Headers = newAndOnGoingAdult;
				nonCashBenefitGroup.HideSubheaders = true;
				nonCashBenefitGroup.UseNonDuplicatedSubtotal = true;
				foreach (var item in Lookups.NonCashBenefits[ReportContainer.Provider])
					nonCashBenefitGroup.Rows.Add(GetReportRowFromLookup(item));
				nonCashBenefitGroup.Rows.Add(new ReportRow { Title = "None Checked", Code = null });
				ReportTableList.Add(nonCashBenefitGroup);

				// Marital Status
				var maritalStatusGroup = new MaritalStatusReportTable("Marital Status", 15);
				maritalStatusGroup.Headers = newAndOnGoingAdult;
				maritalStatusGroup.HideSubheaders = true;
				foreach (var item in Lookups.MaritalStatus[ReportContainer.Provider])
					maritalStatusGroup.Rows.Add(GetReportRowFromLookup(item));
				maritalStatusGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(maritalStatusGroup);

				// Pregnant
				var pregnantGroup = new PregnantReportTable("Pregnant", 16);
				pregnantGroup.Headers = newAndOnGoingAdult;
				pregnantGroup.HideSubheaders = true;
				foreach (var item in Lookups.Pregnant[ReportContainer.Provider])
					pregnantGroup.Rows.Add(GetReportRowFromLookup(item));
				pregnantGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(pregnantGroup);

				// Primary Income Source
				var primaryIncomeGroup = new PrimaryIncomeReportTable("Primary Income Source", 17);
				primaryIncomeGroup.Headers = newAndOnGoingAdult;
				primaryIncomeGroup.UseNonDuplicatedSubtotal = true;
				primaryIncomeGroup.HideSubheaders = true;

				int primaryIncomeOrder = 0;
				primaryIncomeGroup.Rows.Add(new ReportRow { Title = "Unknown", Code = -1, Order = primaryIncomeOrder++ });
				primaryIncomeGroup.Rows.Add(new ReportRow { Title = "No Financial Resources", Code = -2, Order = primaryIncomeOrder++ });
				foreach (var item in Lookups.IncomeSource2[ReportContainer.Provider]) {
					primaryIncomeGroup.Rows.Add(GetReportRowFromLookup(item));
					primaryIncomeGroup.Rows[primaryIncomeOrder++].Order = primaryIncomeOrder;
				}
				primaryIncomeGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null, Order = primaryIncomeOrder });
				ReportTableList.Add(primaryIncomeGroup);

				// Monthly Income 
				var monthlyIncomeGroup = new MonthlyIncomeReportTable("Monthly Income Ranges", 18);
				monthlyIncomeGroup.Headers = newAndOnGoingAdult;
				monthlyIncomeGroup.HideSubheaders = true;
				monthlyIncomeGroup.Rows.Add(new ReportRow { Title = "Less than or equal to $500", Code = 1, Order = 1 });
				monthlyIncomeGroup.Rows.Add(new ReportRow { Title = "Between $500 and $1000", Code = 2, Order = 2 });
				monthlyIncomeGroup.Rows.Add(new ReportRow { Title = "More than $1000", Code = 3, Order = 3 });
				ReportTableList.Add(monthlyIncomeGroup);

				// Number Of Children
				var numberOfChilderenGroup = new NumberOfChildrenReportTable("Number Of Children", 19);
				numberOfChilderenGroup.Headers = newAndOnGoingAdult;
				numberOfChilderenGroup.HideSubheaders = true;
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "None", Code = 0, Order = 1 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "One", Code = 1, Order = 2 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Two", Code = 2, Order = 3 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Three", Code = 3, Order = 4 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Four", Code = 4, Order = 5 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Five", Code = 5, Order = 6 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Six", Code = 6, Order = 7 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Seven", Code = 7, Order = 8 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Eight or more", Code = 8, Order = 9 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null, Order = 10 });
				ReportTableList.Add(numberOfChilderenGroup);

				//////// Child Client Information ///////
				// Education
				var childEducationGroup = new ChildEducationReportTable("Education", 20);
				childEducationGroup.Headers = newAndOnGoingChild;
				childEducationGroup.HideSubheaders = true;
				childEducationGroup.PreHeader = "Child Client Information";
				foreach (var item in Lookups.School[ReportContainer.Provider])
					childEducationGroup.Rows.Add(GetReportRowFromLookup(item));
				childEducationGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(childEducationGroup);

				// Child Abuse
				var childAbuseGroup = new ChildAbuseReportTable("Child Abuse", 21);
				childAbuseGroup.Headers = newAndOnGoingChild;
				childAbuseGroup.HideSubheaders = true;
				childAbuseGroup.HideSubtotal = true;
				childAbuseGroup.Rows.Add(new ReportRow { Title = "Investigations", Code = 1, Order = 1 });
				childAbuseGroup.Rows.Add(new ReportRow { Title = "DCFS Case Open", Code = 2, Order = 2 });
				ReportTableList.Add(childAbuseGroup);

				// Custody
				var custodyReportTable = new CustodyReportTable("Custody", 22);
				custodyReportTable.Headers = newAndOnGoingChild;
				custodyReportTable.HideSubheaders = true;
				foreach (var item in Lookups.ChildCustody[ReportContainer.Provider])
					custodyReportTable.Rows.Add(GetReportRowFromLookup(item));
				custodyReportTable.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(custodyReportTable);

				// Lives With
				var livesWithGroup = new LivesWithReportTable("Lives With", 23);
				livesWithGroup.Headers = newAndOnGoingChild;
				livesWithGroup.HideSubheaders = true;
				foreach (var item in Lookups.ChildLivesWith[ReportContainer.Provider])
					livesWithGroup.Rows.Add(GetReportRowFromLookup(item));
				livesWithGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(livesWithGroup);
			} else if (ReportContainer.Provider == Provider.SA) { ////////////////// SA ONLY //////////////////

				// Employment
				var employmentGroup = new EmploymentReportTable("Employment", 10);
				employmentGroup.Headers = newAndOnGoingAllClientTypes;
				foreach (var item in Lookups.EmploymentType[ReportContainer.Provider])
					employmentGroup.Rows.Add(GetReportRowFromLookup(item));
				employmentGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(employmentGroup);

				// Education
				var educationGroup = new SAEducationReportTable("Education", 11);
				educationGroup.Headers = newAndOnGoingAllClientTypes;
				foreach (var item in Lookups.Education[ReportContainer.Provider])
					educationGroup.Rows.Add(GetReportRowFromLookup(item));
				educationGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(educationGroup);

				// Health insurance
				var healthInsuranceGroup = new HealthInsuranceReportTable("Health Insurance", 12);
				healthInsuranceGroup.Headers = newAndOnGoingAllClientTypes;
				foreach (var item in Lookups.HealthInsurance[ReportContainer.Provider])
					healthInsuranceGroup.Rows.Add(GetReportRowFromLookup(item));
				healthInsuranceGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(healthInsuranceGroup);

				// Marital Status
				var maritalStatusGroup = new MaritalStatusReportTable("Marital Status", 14);
				maritalStatusGroup.Headers = newAndOnGoingAllClientTypes;
				foreach (var item in Lookups.MaritalStatus[ReportContainer.Provider])
					maritalStatusGroup.Rows.Add(GetReportRowFromLookup(item));
				maritalStatusGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(maritalStatusGroup);

				// Pregnant
				var pregnantGroup = new PregnantReportTable("Pregnant", 15);
				pregnantGroup.Headers = newAndOnGoingAllClientTypes;
				foreach (var item in Lookups.Pregnant[ReportContainer.Provider])
					pregnantGroup.Rows.Add(GetReportRowFromLookup(item));
				pregnantGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(pregnantGroup);

				// Primary Income Source
				var primaryIncomeGroup = new PrimaryIncomeReportTable("Primary Income Source", 16);
				primaryIncomeGroup.Headers = newAndOnGoingAllClientTypes;
				foreach (var item in Lookups.IncomeSource[ReportContainer.Provider])
					primaryIncomeGroup.Rows.Add(GetReportRowFromLookup(item));
				primaryIncomeGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(primaryIncomeGroup);

				// College Student
				var studentGroup = new CollegeStudentReportTable("College Student", 17);
				studentGroup.Headers = newAndOnGoingAllClientTypes;
				studentGroup.HideSubtotal = true;
				studentGroup.Rows.Add(new ReportRow { Title = "Yes", Code = null });
				ReportTableList.Add(studentGroup);
			} else if (ReportContainer.Provider == Provider.CAC) { ////////////////// CAC ONLY //////////////////
				//////// Child Victim Client Information ///////
				// Health insurance
				var healthInsuranceGroup = new HealthInsuranceReportTable("Health Insurance", 9);
				healthInsuranceGroup.Headers = newAndOnGoingChildVictim;
				healthInsuranceGroup.HideSubheaders = true;
				healthInsuranceGroup.PreHeader = "Other Client Information (Victim)";
				foreach (var item in Lookups.HealthInsurance[ReportContainer.Provider])
					healthInsuranceGroup.Rows.Add(GetReportRowFromLookup(item));
				healthInsuranceGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(healthInsuranceGroup);

				// Pregnant
				var pregnantGroup = new PregnantReportTable("Pregnant", 10);
				pregnantGroup.Headers = newAndOnGoingChildVictim;
				pregnantGroup.HideSubheaders = true;
				foreach (var item in Lookups.Pregnant[ReportContainer.Provider])
					pregnantGroup.Rows.Add(GetReportRowFromLookup(item));
				pregnantGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(pregnantGroup);

				// Custody
				var custodyReportTable = new CustodyReportTable("Custody", 11);
				custodyReportTable.Headers = newAndOnGoingChildVictim;
				custodyReportTable.HideSubheaders = true;
				foreach (var item in Lookups.ChildCustody[ReportContainer.Provider])
					custodyReportTable.Rows.Add(GetReportRowFromLookup(item));
				custodyReportTable.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(custodyReportTable);

				//////// Non-Offending CareTaker Client Information ///////
				// Employment
				var employmentGroup = new EmploymentReportTable("Employment", 12);
				employmentGroup.Headers = newAndOnGoingCareTaker;
				employmentGroup.HideSubheaders = true;
				employmentGroup.PreHeader = "Other Client Information(Non-Offending Caretaker)";
				foreach (var item in Lookups.EmploymentType[ReportContainer.Provider])
					employmentGroup.Rows.Add(GetReportRowFromLookup(item));
				employmentGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(employmentGroup);

				// Marital Status
				var maritalStatusGroup = new MaritalStatusReportTable("Marital Status", 13);
				maritalStatusGroup.Headers = newAndOnGoingCareTaker;
				maritalStatusGroup.HideSubheaders = true;
				foreach (var item in Lookups.MaritalStatus[ReportContainer.Provider])
					maritalStatusGroup.Rows.Add(GetReportRowFromLookup(item));
				maritalStatusGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(maritalStatusGroup);

				// Number Of Children
				var numberOfChilderenGroup = new NumberOfChildrenReportTable("Number Of Children (in home)", 14);
				numberOfChilderenGroup.Headers = newAndOnGoingCareTaker;
				numberOfChilderenGroup.HideSubheaders = true;
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "None", Code = 0, Order = 1 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "One", Code = 1, Order = 2 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Two", Code = 2, Order = 3 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Three", Code = 3, Order = 4 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Four", Code = 4, Order = 5 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Five", Code = 5, Order = 6 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Six", Code = 6, Order = 7 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Seven", Code = 7, Order = 8 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Eight or more", Code = 8, Order = 9 });
				numberOfChilderenGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null, Order = 10 });
				ReportTableList.Add(numberOfChilderenGroup);

				// Relationship to victim
				var relationshipGroup = new CACRelationshipToVictimReportTable("Relationship to Victim", 15);
				relationshipGroup.Headers = GetHeadersForRelationshipReportTable();
				ReportTableList.Add(relationshipGroup);
			}
		}

		private List<ReportTableHeader> GetHeadersForRelationshipReportTable() {
			var headers = new List<ReportTableHeader>();
			var subheaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.NonOffendingCaretaker, Title = "Non Offending Caretaker" }, new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.ChildNonVictim, Title = "Child (Non-victim)" }, new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.CACSignifigantOther, Title = "CAC Significant Other" }, new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = "Total" } };
			headers.Add(new ReportTableHeader { Code = ReportTableHeaderEnum.New, Title = "New", SubHeaders = subheaders });
			headers.Add(new ReportTableHeader { Code = ReportTableHeaderEnum.Ongoing, Title = "Ongoing", SubHeaders = subheaders });
			headers.Add(new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = "Total", SubHeaders = subheaders });
			return headers;
		}
	}

	public class ClientInformationDemographicsLineItem {
		public ClientInformationDemographicsLineItem() {
			ClientShelterTypeIDs = new List<int>();
			RaceIDs = new List<int>();
			PrimaryIncomeSources = new List<IncomeSource>();
		}

		public string ClientCode { get; set; }
		public int? ClientID { get; set; }
		public int? CaseID { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? ClientTypeID { get; set; }
		public DateTime? FirstContactDate { get; set; }
		public string HouseholdID { get; set; }
		public IEnumerable<int> ClientShelterTypeIDs { get; set; }
		public int? GenderIdentityID { get; set; }
		public int? SexualOrientationID { get; set; }
		public int? EthnicityID { get; set; }
		public IEnumerable<int> RaceIDs { get; set; }
		public int? RaceId { get; set; }
		public int? AgeAtFirstContact { get; set; }
		public int? VetStatusID { get; set; }
		public int? EmploymentID { get; set; }
		public int? EducationID { get; set; }
		public int? SchoolID { get; set; }
		public int? HealthInsuranceID { get; set; }
		public bool? NCB_FoodBenefit { get; set; }
		public bool? NCB_OtherSource { get; set; }
		public bool? NCB_OtherTANF { get; set; }
		public bool? NCB_PublicHousing { get; set; }
		public bool? NCB_SpecSuppNutr { get; set; }
		public bool? NCB_TANFChildCare { get; set; }
		public bool? NCB_TANFTrans { get; set; }
		public bool? NCB_NoBenefit { get; set; }
		public bool? NCB_Unknown { get; set; }
		public bool? Ins_Medicaid { get; set; }
		public bool? Ins_Medicare { get; set; }
		public bool? Ins_StateChildHealth { get; set; }
		public bool? Ins_VetAdminMed { get; set; }
		public bool? Ins_Private { get; set; }
		public bool? Ins_NoHealthIns { get; set; }
		public bool? Ins_Unknown { get; set; }
		public int? MaritalStatusID { get; set; }
		public int? PregnantID { get; set; }
		public IEnumerable<IncomeSource> IncomeSources { get; set; }
		public IEnumerable<IncomeSource> PrimaryIncomeSources { get; set; }
		public decimal? MonthlyIncome { get; set; }
		public int? NumberOfChildren { get; set; }
		public int? DCFSInvestigation { get; set; }
		public int? DCFSOpen { get; set; }
		public int? CustodyID { get; set; }
		public int? LivesWithID { get; set; }
		public int? SAPrimaryIncomeSource { get; set; }
		public int? IsCollegeStudent { get; set; }
		public int? RelationshipToVictimID { get; set; }
	}

	public class IncomeSource {
		public int? IncomeID { get; set; }
		public decimal? Amount { get; set; }
	}
}