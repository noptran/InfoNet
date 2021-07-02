using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;

namespace Infonet.Web.ViewModels {
	public class CommunityServicesViewModel : IRevisable {
		public int? ICS_ID { get; set; }

		[Display(Name = "Community/Institutional Service")]
		[Required]
		[Lookup("CommunityServices")]
		public int ProgramID { get; set; }

		public string ProgramName { get; set; }

		[Required]
		[WholeNumber]
		[Range(0, 9999999)]
		[Display(Name = "Number of Presentations/Contacts")]
		public int? NumOfSession { get; set; }

		[Required]
		[Range(0, 999)]
		[Display(Name = "Total Hours")]
		[QuarterIncrement]
		public double? Hours { get; set; }

		[Required]
		[WholeNumber]
		[Range(0, 999999)]
		[Display(Name = "Number of Participants")]
		public int? ParticipantsNum { get; set; }

		[Required]
		[BetweenNineteenNinetyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date")]
		public DateTime? PDate { get; set; }

        [MaxLength(200, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Comments")]
		public string Comment_Act { get; set; }

		[Display(Name = "Agency")]
		public int? AgencyID { get; set; }

		[DataType(DataType.Text)]
        [MaxLength(90, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string Location { get; set; }

		[Help("Select the county where the presentation/contact was made.")]
		[Help(Provider.CAC, "Select the county where the presentation/contact was made. If the presentation/contact occurred in another state, change the state field and the county drop-down menu will reflect the counties in that state.")]
		[Display(Name = "County")]
		public int? CountyID { get; set; }

		[Display(Name = "State")]
		public int? StateID { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public string ReturnURL { get; set; }

		public virtual IList<ProgramDetailStaff> ProgramDetailStaff { get; set; }

		public int saveAddNew { get; set; }
	}
}