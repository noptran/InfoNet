using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum ReportColumnSelectionsEnum {
		[Display(Name ="Client ID")]
		ClientCode,
		[Display(Name = "Case ID")]
		CaseID,
		[Display(Name = "Client Type")]
		ClientType,
		[Display(Name = "First Contact Date")]
		FirstContactDate,
		[Display(Name = "Age")]
		Age,
		[Display(Name = "Gender")]
		Gender,
		[Display(Name = "Ethnicity")]
		Ethnicity,
		[Display(Name = "Race")]
		Race,
        [Display(Name = "Sexual Orientation")]
        SexualOrientation,
        [Display(Name = "Town")]
		Town,
		[Display(Name = "Township")]
		Township,
		[Display(Name = "County")]
		County,
		[Display(Name = "State")]
		State,
		[Display(Name = "Zip Code")]
		ZipCode,
		[Display(Name = "Service Name")]
		ServiceName,
		[Display(Name = "Service Date")]
		ServiceDate,
		[Display(Name = "Service Hours")]
		ServiceHours,
		[Display(Name = "Shelter Begin Date")]
		ShelterBeginDate,
		[Display(Name = "Shelter End Date")]
		ShelterEndDate,
		[Display(Name = "Staff Name")]
		Staff,
		[Display(Name = "Hotline Call Type")]
		HotlineCallType,
		[Display(Name = "Hotline Call Date")]
		HotlineCallDate,
		[Display(Name = "Hotline Call Contacts")]
		HotlineCallContacts,
		[Display(Name = "Hotline Call Time")]
		HotlineCallTime,
		[Display(Name = "Contact Type")]
		ContactType,
		[Display(Name = "Contact Date")]
		ContactDate,
		[Display(Name = "Number Of Contacts")]
		NumOfContacts,
		[Display(Name = "Contact Time")]
		ContactTime,
		[Display(Name = "Activity")]
		Activity,
		[Display(Name = "Date")]
		Date,
		[Display(Name = "Conduct Hours")]
		ConductHours,
		[Display(Name = "Travel Hours")]
		TravelHours,
		[Display(Name = "Prepare Hours")]
		PrepareHours,
		[Display(Name = "Reason")]
		Reason,
		[Display(Name = "Date Issued")]
		DateIssued,
		[Display(Name = "Date of Expiration")]
		DateExpired,
		[Display(Name = "Date of Last Service")]
		DateOfLastService,
		[Display(Name = "Comment")]
		Comment,
		[Display(Name = "Client Status")]
		ClientStatus,
		[Display(Name = "Client Info")]
		ClientInfo,
		[Display(Name = "No. Of Adult Victims")]
		TurnAwayNumOfAdultVictims,
		[Display(Name = "No. of Children")]
		TurnAwayNumOfChildren,
		[Display(Name = "No. of Families")]
		TurnAwayNumOfFamily,
		[Display(Name = "Referral Made to Another Shleter?")]
		TurnAwayReferralMade,
		[Display(Name = "Original Order Type")]
		OriginalOpType,
		[Display(Name = "Media/Publication Type")]
		MediaPublicationType,
		[Display(Name = "Title")]
		Title,
		[Display(Name = "Number of Segments")]
		NumOfSegments,
		[Display(Name = "Staff Preparation Hours")]
		StaffPrepHours,
		[Display(Name = "Event Type")]
		EventType,
		[Display(Name = "Event Name")]
		EventName,
		[Display(Name = "Event Hours")]
		EventHrs,
		[Display(Name = "People Reached")]
		PeopleReached,
		[Display(Name = "Staff Conduct Hours")]
		StaffConductHours,
		[Display(Name = "Staff Travel Hours")]
		StaffTravelHours,
		[Display(Name = "Location")]
		Location,
		[Display(Name = "Agency")]
		Agency,
		[Display(Name = "Staff Presentation Hrs")]
		StaffPresentationHrs,
		[Display(Name = "Total Presentation/ Contact Hrs")]
		PresentationHrs,
		[Display(Name = "Number of Presentations/ Contacts")]
		NumOfPresentations,
		[Display(Name = "Total Number of Participants")]
		NumOfParticipants       
    }
}