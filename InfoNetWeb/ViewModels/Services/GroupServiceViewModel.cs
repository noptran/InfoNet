using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;
using Infonet.Data;
using Infonet.Data.Models.Services;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Services {
	public class GroupServiceViewModel : PagedListPagination, IRevisable, IValidatableObject {
		public GroupServiceViewModel() {
			StartDate = DateTime.Today.AddMonths(-3).Date;
			EndDate = DateTime.Today.Date;
			Range = "13";
			PageSize = 10;
			ClientSearchViewModel = new ClientSearchViewModel(true, true) {
				ServiceStart = DateTime.Today.AddMonths(-3).Date,
				ServiceEnd = DateTime.Today.Date,
				ServiceRange = "13"
			};
		}

		public int? ICS_ID { get; set; }

		[Display(Name = "Group Service")]
		[Required]
		public int ProgramID { get; set; }

		[Required]
		[Range(0, 200)]
		[Display(Name = "Number of Sessions ")]
		[WholeNumber]
		public int? NumOfSession { get; set; }

		[Required]
		[Range(0, 999)]
		[Display(Name = "Hours in Session")]
		[QuarterIncrement]
		public double? Hours { get; set; }

		[Required]
		[Range(0, 999)]
		[Display(Name = "Number of Attendees")]
		[WholeNumber]
		public int? ParticipantsNum { get; set; }

		[Required]
		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date")]
		public DateTime? PDate { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public int saveAddNew { get; set; }

		public string ReturnURL { get; set; }

		//Date Range for Attendee Search by Service
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? ServiceStart { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? ServiceEnd { get; set; }

		public string ServiceRange { get; set; } // Service Date Range for Search Attendees 

		//Date Range for Attendee Search by First Contact Dtae
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? FCD_StartDate { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? FCD_EndDate { get; set; }

		public ClientSearchViewModel ClientSearchViewModel { get; set; }

		public virtual IList<ProgramDetailStaff> ProgramDetailStaff { get; set; }
		public virtual IList<AttendeeViewModel> Attendees { get; set; }

		public IPagedList<GroupServiceSearchResult> GroupServiceList { get; set; }

		public class GroupServiceSearchResult {
			public int? ICS_ID { get; set; }

			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			[BetweenNineteenSeventyToday]
			public DateTime? PDate { get; set; }

			public double? Hours { get; set; }
			public int? NumOfSession { get; set; }
			public int? ParticipantsNum { get; set; }
			public string ProgramName { get; set; }
			public string employeeNames { get; set; }
			public virtual IList<ProgramDetailStaff> ProgramDetailStaff { get; set; }
		}
		
		//KMS DO ideally, this belongs in ProgramDetail and wouldn't then need an InfonetServerContext
		IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) {
			var results = new List<ValidationResult>();
			if (Attendees != null && PDate != null)
				using (var db = new InfonetServerContext())
					for (int i = 0; i < Attendees.Count; i++) {
						var clientId = Attendees[i].ServiceDetailOfClient.ClientID;
						var caseId = Attendees[i].ServiceDetailOfClient.CaseID;
						var firstContactDate = db.T_ClientCases.Where(t => t.ClientId == clientId && t.CaseId == caseId).Select(s => s.FirstContactDate).SingleOrDefault();
						if (PDate < firstContactDate)
							results.Add(new ValidationResult("This group service session occured before Client " + Attendees[i].ClientCode + "'s First Contact Date.  You must edit the group service session date or Client " + Attendees[i].ClientCode + " Case " + Attendees[i].ServiceDetailOfClient.CaseID + "'s First Contact Date before adding this client case to the group service session.", new[] { "Attendees[" + i + "].ServiceDetailOfClient.CaseID" }));
					}
			return results;
		}

		public class AttendeeViewModel {
			[Display(Name = "Client ID")]
			public string ClientCode { get; set; }

			public List<SelectListItem> Cases { get; set; }
			public virtual ServiceDetailOfClient ServiceDetailOfClient { get; set; }
		}
	}
}