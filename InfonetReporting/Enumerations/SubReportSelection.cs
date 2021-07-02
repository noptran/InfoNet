using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
    public enum SubReportSelection {
        [Display(Name = "Basic Demographics")]
        StdRptClientInformationBasicDemographics = 1,
        [Display(Name = "Referral Source")]
        StdRptClientInformationReferralSource = 2,
        [Display(Name = "Special Needs")]
        StdRptClientInformationSpecialNeeds = 3,
        [Display(Name = "Presenting Issues")]
        StdRptClientInformationPresentingIssues = 4,
        [Display(Name = "Aggregate Client Information")]
        StdRptClientInformationAggregateClientInformation = 5,
        [Display(Name = "Residence/Destination Information")]
        StdRptClientInformationResidenceDestinationInformation = 6,
        [Display(Name = "Direct Client Services")]
        StdRptServiceProgramsDirectClientServices = 7,
        [Display(Name = "Community, Institutional and Group Services")]
        StdRptServiceProgramsCommunityInstitutionalGroupServices = 8,
        [Display(Name = "Hotline/Information Referral")]
        StdRptServiceProgramsHotlineInformationReferral = 9,
        [Display(Name = "Non-Client Crisis Intervention")]
        StdRptServiceProgramsNonClientCrisisIntervention = 10,
        [Display(Name = "Show Demographics")]
        StdRptServiceProgramsNonClientCrisisInterventionDemographics = 11,
        [Display(Name = "Client Services Report")]
        StdRptServiceProgramsHudHmisDirectServices = 12,
        [Display(Name = "Group Services")]
        StdRptServiceProgramsHudHmisGroupServices = 13,
        [Display(Name = "Volunteer Service Information")]
        StdRptServiceProgramsVolunteerServiceInformation = 14,
        [Display(Name = "Turn Away Information Report")]
        StdRptServiceProgramsHudHmisTurnAway = 15,
        [Display(Name = "Service Outcomes Service Report")]
        StdRptServiceProgramsServiceOutcomesServiceReport = 16,
        [Display(Name = "HUD/HMIS Service Report")]
        StdRptServiceProgramsHudHmisServiceReport = 17,
        [Display(Name = "Offenders")]
        StdRptMedicalCJOffenders = 18,
        [Display(Name = "Medical System Involvement")]
        StdRptMedicalCJMedicalSystemInvolvement = 19,
        [Display(Name = "Police Involvement")]
        StdRptMedicalCJPoliceInvolvement = 20,
        [Display(Name = "Prosecution Involvement")]
        StdRptMedicalCJProsecutionInvolvement = 21,
        [Display(Name = "Order of Protection")]
        StdRptMedicalCJOrderOfProtection = 22,
        [Display(Name = "DCFS Allegations")]
        StdRptInvestigationDCFSAllegations = 23,
        [Display(Name = "Abuse Neglect Petitions")]
        StdRptInvestigationAbuseNeglectPetitions = 24,
        [Display(Name = "Multi-Disciplinary Team")]
        StdRptInvestigationMultiDisciplinaryTeam = 25,
        [Display(Name = "Victim Sensitive Interview")]
        StdRptInvestigationVictimSensitiveInterview = 26,
        [Display(Name = "Medical")]
        StdRptInvestigationMedical = 27,
        [Display(Name = "Client Detail Information", ShortName = "ClientDetail")]
        MngRptClientClientDetail = 28,
        [Display(Name = "Child Behavorial Issues", ShortName = "ChildBehavioral")]
        MngRptClientChildBehavioral = 29,
        [Display(Name = "Income Source Management Report", ShortName = "IncomeSource")]
        MngRptClientIncomeSourceManagement = 30,
        [Display(Name = "Staff/Client Service Information", ShortName = "StaffClientService")]
        MngRptStaffServiceServiceInformation = 31,
        [Display(Name = "Staff/Community, Institutional & Group Services Information", ShortName = "StaffCommInstGroup")]
        MngRptStaffServiceCommunityGroup = 32,
        [Display(Name = "Staff/Media/Publication Information", ShortName = "StaffMediaPub")]
        MngRptStaffServiceMediaPublication = 33,
        [Display(Name = "Staff/Event Information", ShortName = "StaffEvent")]
        MngRptStaffServiceEvent = 34,
        [Display(Name = "Staff/Event & Media/Publication Information", ShortName = "StaffEventMediaPublication")]
        MngRptStaffServiceEventMediaPublication = 35,
        [Display(Name = "Staff/Hotline Call Information", ShortName = "StaffHotline")]
        MngRptStaffServiceHotline = 36,
        [Display(Name = "Staff/Non-Client Crisis Intervention Information", ShortName = "StaffNonClientCrisis")]
        MngRptStaffServiceCrisisIntervention = 37,
        [Display(Name = "Staff Report", ShortName = "StaffReport")]
        MngRptStaffServiceStaffReport = 38,
        [Display(Name = "Other Staff Activity Report", ShortName = "OtherStaffActivity")]
        MngRptStaffServiceOtherStaffActivity = 39,
        [Display(Name = "Turn Away Information", ShortName = "TurnAway")]
        MngRptStaffServiceTurnAway = 40,
        [Display(Name = "Cancellation/No Show Information", ShortName = "CancellationNoShow")]
        MngRptStaffServiceCancellation = 41,
        [Display(Name = "Order of Protection Report", ShortName = "OrderOfProtection")]
        MngRptOtherOrderOfProtection = 42,
        [Display(Name = "Clients Without Service Record")]
        ExcRptClientsWithoutServiceRecord = 43,
        [Display(Name = "First Contact Date Later Than Service Date")]
        ExcRptFirstContactDateLaterThanServiceDate = 44,
        [Display(Name = "Orders of Protection Without Exipration Date")]
        ExcRptOrdersOfProtectionWithoutExpirationDate = 45,
        [Display(Name = "Open Client Cases")]
        ExcRptOpenClientCases = 46,
        [Display(Name = "Clients Without Presenting Issues")]
        ExcRptClientsWithoutPresentingIssues = 47,
        [Display(Name = "Open & Lengthy Shelter Dates")]
        ExcRptLengthyShelterUse = 48,
        [Display(Name = "Clients With Unknown/Not Reported/Unassigned Data Fields")]
        ExcRptClientsWithUNRUFields = 49,
        [Display(Name = "Clients Without Offender Information")]
        ExcRptClientsWithoutOffenderInformation = 50
    }
}