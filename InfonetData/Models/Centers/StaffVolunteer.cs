using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Infonet.Core.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Models.Centers {
	public class StaffVolunteer : IRevisable {
		public StaffVolunteer() {
			PhoneHotlines = new List<PhoneHotline>();
			Volunteers = new List<StaffVolunteer>();
			Cancellations = new List<Cancellation>();
			FundServiceProgramsOfStaff = new List<FundServiceProgramOfStaff>();
			ServiceDetailsOfClient = new List<ServiceDetailOfClient>();
			EventDetailStaff = new List<EventDetailStaff>();
			OtherStaffActivities = new List<OtherStaffActivity>();
			ProgramDetailStaff = new List<ProgramDetailStaff>();
			PublicationDetailStaff = new List<PublicationDetailStaff>();
		}

		public int SvId { get; set; }

		public int CenterId { get; set; }

		#region obsolete
		[Obsolete]
		public string SocSec { get; set; }
		#endregion

		//KMS DO StringTrim on these (in ViewModel?)
		//KMS DO StringTrim elsewhere?
		public string LastName { get; set; }

		public string FirstName { get; set; }

		[Lookup("Sex")]
		public int? SexId { get; set; }

		[Lookup("Race")]
		public int? RaceId { get; set; }

		#region obsolete
		[Obsolete]
		public DateTime? BirthDate { get; set; }
		#endregion

		[Lookup("PersonnelType")]
		public int? PersonnelTypeId { get; set; }

		#region obsolete
		[Obsolete]
		public string PersonnelType { get; set; }
		#endregion

		public string Title { get; set; }

		public bool CollegeUnivStudent { get; set; }

		#region obsolete
		[Obsolete]
		public string Greeting { get; set; }
		#endregion

		public string Department { get; set; }

		#region obsolete
		[Obsolete]
		public string Address { get; set; }

		[Obsolete]
		public string City { get; set; }

		[Obsolete]
		public int? StateID { get; set; }

		[Obsolete]
		public string ZipCode { get; set; }
		#endregion

		public string WorkPhone { get; set; }

		#region obsolete
		[Obsolete]
		public string HomePhone { get; set; }
		#endregion

		public string Email { get; set; }

		#region obsolete
		[Obsolete]
		public string EmerContact { get; set; }
		#endregion

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Start Date")]
		public DateTime? StartDate { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Termination Date")]
		public DateTime? TerminationDate { get; set; }

		//KMS DO obsolete?
		public string Type { get; set; }

		public int? SupervisorId { get; set; }

		#region obsolete
		[Obsolete]
		public int? NumInGroup { get; set; }
		#endregion

		public DateTime? RevisionStamp { get; set; }

		public int TypeId { get; set; }

		public virtual Center Center { get; set; }

		public virtual ICollection<PhoneHotline> PhoneHotlines { get; set; }

		public virtual ICollection<StaffVolunteer> Volunteers { get; set; }

		public virtual StaffVolunteer Supervisor { get; set; }

		public virtual ICollection<Cancellation> Cancellations { get; set; }

		public virtual ICollection<FundServiceProgramOfStaff> FundServiceProgramsOfStaff { get; set; }

		public virtual ICollection<ServiceDetailOfClient> ServiceDetailsOfClient { get; set; }

		public virtual ICollection<EventDetailStaff> EventDetailStaff { get; set; }

		public virtual ICollection<OtherStaffActivity> OtherStaffActivities { get; set; }

		public virtual ICollection<ProgramDetailStaff> ProgramDetailStaff { get; set; }

		public virtual ICollection<PublicationDetailStaff> PublicationDetailStaff { get; set; }

		[NotMapped]
		public StaffType TypeOfStaff {
			get { return (StaffType)TypeId; }
			set { TypeId = (int)value; }
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public enum StaffType {
			[Display(Name = "Paid Staff")] Staff = 1,
			Volunteer = 2,
			Unknown = 0
		}
	}
}