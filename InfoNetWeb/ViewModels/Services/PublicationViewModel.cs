using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Models.Services;

namespace Infonet.Web.ViewModels.Services {
	public class PublicationViewModel : IRevisable {
		public int? ICS_ID { get; set; }

		[Required]
        [MaxLength(99, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Media/Publication Title")]
		public string Title { get; set; }

		[Display(Name = "Media/Publication Type")]
		[Required]
		public int ProgramID { get; set; }

		[Required]
		[WholeNumber]
		[Range(0, 999999)]
		[Display(Name = "Number of Publications or Media Segments")]
		public int? NumOfBrochure { get; set; }

		[Required]
		[Range(0, 99999)]
		[Display(Name = "Prepare Hours")]
		[QuarterIncrement]
		public double? PrepareHours { get; set; }

		[Required]
		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Publication Date")]
		public DateTime? PDate { get; set; }

        [MaxLength(200, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Comments")]
		public string Comment_Pub { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public string ReturnURL { get; set; }
		public virtual IList<PublicationDetailStaff> PublicationDetailStaff { get; set; }

		public int saveAddNew { get; set; }
	}
}