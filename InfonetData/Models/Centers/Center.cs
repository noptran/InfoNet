using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Investigations;
using Infonet.Data.Models.Services;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Models.Centers {
	[BindHint(Include = "Address, Zipcode, StateID, CountyID, CityID, Email, Telephone, Fax, FederalEmployerID, BoardMemberNum, EmpNumber, LegisDistrict, JudicialDistrict, ServiceArea, Population")]
	public class Center : IRevisable {
		public Center() {
			Investigations = new List<Investigation>();
			Satellites = new List<Center>();
			Clients = new List<Client>();
			Contacts = new List<Contact>();
			FundingDates = new List<FundingDate>();
			PhoneHotlines = new List<PhoneHotline>();
			StaffVolunteers = new List<StaffVolunteer>();
			EventDetails = new List<EventDetail>();
			ProgramDetails = new List<ProgramDetail>();
			PublicationDetails = new List<PublicationDetail>();
			ServiceDetailsOfClients = new List<ServiceDetailOfClient>();
			TLU_Codes_FundingSource = new List<TLU_Codes_FundingSource>();
			TLU_Codes_OtherStaffActivity = new List<TLU_Codes_OtherStaffActivity>();
			Administrators = new List<CenterAdministrators>();
			ClientReferralDetails = new List<ClientReferralDetail>();
			HivMentalSubstance = new List<HivMentalSubstance>();
			ServiceOutcomes = new List<ServiceOutcome>();
			TurnAwayService = new List<TurnAwayService>();
			VictimSensitiveInterviews = new List<VictimSensitiveInterview>();
		}

		public int CenterID { get; set; }

		public int? ParentCenterID { get; set; }

		public int ProviderID { get; set; }

		public string CenterName { get; set; }

		//[Required]
		//KMS DO [StringTrim]
		[Display(Name = "Address")]
		[MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string Address { get; set; }

		#region obsolete
		[Obsolete]
		public string City { get; set; }
		#endregion

		//[Required]
		[Display(Name = "Zip Code")]
		[StringLength(15, MinimumLength = 5, ErrorMessage = "The Zip Code must have a minimum of five(5) digits.")]
		public string Zipcode { get; set; }

		//[Required]
		[Display(Name = "County")]
		public int? CountyID { get; set; }

		[Display(Name = "Number of Employees (FTEs)")]
		[WholeNumber]
		[Range(0, 999, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		public int? EmpNumber { get; set; }

		#region obsolete
		[Obsolete]
		public decimal? AgeAnnBudget { get; set; }

		[Obsolete]
		public decimal? ProgAnnBudget { get; set; }
		#endregion

		[Display(Name = "Number of Board Members")]
		[WholeNumber]
		[Range(0, 99, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
		public short? BoardMemberNum { get; set; }

		[Display(Name = "Legislative District")]
		[MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string LegisDistrict { get; set; }

		[Display(Name = "Judicial District")]
		[MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string JudicialDistrict { get; set; }

		[Display(Name = "Service Area")]
		[MaxLength(100, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string ServiceArea { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N0}")]
		[Display(Name = "Population")]
		[WholeNumber]
		[Range(0, 99999999)]
		public double? Population { get; set; }

		[Display(Name = "Fax Number")]
		[RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Invalid Fax Number. Format must match (xxx) xxx-xxxx")]
		public string Fax { get; set; }

		[Display(Name = "Federal Employer ID")]
		[MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string FederalEmployerID { get; set; }

		//[Required]
		[Display(Name = "Email Address")]
		[MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		//[EmailAddress(ErrorMessage = "Invalid Email Address")]
		[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		//[Required]
		[Display(Name = "Phone Number")]
		[RegularExpression(@"^\(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Invalid Phone Number. Format must match (xxx) xxx-xxxx")]
		public string Telephone { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public bool? IsRealCenter { get; set; }

		public string DirectorEmail { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime? TerminationDate { get; set; }

		//[Required]
		[Display(Name = "State")]
		public int? StateID { get; set; }

		//[Required]
		[Display(Name = "City")]
		public int? CityID { get; set; }

		public bool? ShelterStatus { get; set; }

		public virtual ICollection<Investigation> Investigations { get; set; }

		public virtual ICollection<Center> Satellites { get; set; }

		public virtual Center Parent { get; set; }

		public virtual ICollection<Client> Clients { get; set; }

		public virtual ICollection<Contact> Contacts { get; set; }

		public virtual ICollection<FundingDate> FundingDates { get; set; }

		public virtual ICollection<PhoneHotline> PhoneHotlines { get; set; }

		public virtual ICollection<StaffVolunteer> StaffVolunteers { get; set; }

		public virtual ICollection<EventDetail> EventDetails { get; set; }

		public virtual ICollection<ProgramDetail> ProgramDetails { get; set; }

		public virtual ICollection<PublicationDetail> PublicationDetails { get; set; }

		public virtual ICollection<ServiceDetailOfClient> ServiceDetailsOfClients { get; set; }

		public virtual ICollection<TLU_Codes_FundingSource> TLU_Codes_FundingSource { get; set; }

		public virtual ICollection<TLU_Codes_OtherStaffActivity> TLU_Codes_OtherStaffActivity { get; set; }

		public virtual ICollection<CenterAdministrators> Administrators { get; set; }

		public virtual ICollection<ClientReferralDetail> ClientReferralDetails { get; set; }

		public virtual ICollection<HivMentalSubstance> HivMentalSubstance { get; set; }

		public virtual ICollection<ServiceOutcome> ServiceOutcomes { get; set; }

		public virtual ICollection<TurnAwayService> TurnAwayService { get; set; }

		public virtual ICollection<VictimSensitiveInterview> VictimSensitiveInterviews { get; set; }

		public bool IsUnchanged(Center obj) {
			return obj != null &&
					Address == obj.Address &&
					Zipcode == obj.Zipcode &&
					StateID == obj.StateID &&
					CountyID == obj.CountyID &&
					CityID == obj.CityID &&
					Email == obj.Email &&
					Telephone == obj.Telephone &&
					Fax == obj.Fax &&
					FederalEmployerID == obj.FederalEmployerID &&
					BoardMemberNum == obj.BoardMemberNum &&
					EmpNumber == obj.EmpNumber &&
					LegisDistrict == obj.LegisDistrict &&
					JudicialDistrict == obj.JudicialDistrict &&
					ServiceArea == obj.ServiceArea &&
					Population == obj.Population;
		}
	}
}