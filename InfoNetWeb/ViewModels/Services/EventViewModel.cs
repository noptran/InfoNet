using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Models.Services;

namespace Infonet.Web.ViewModels.Services {
	public class EventViewModel : IRevisable {
		public int? ICS_ID { get; set; }

		[Display(Name = "Event Type")]
		[Required]
		public int ProgramID { get; set; }

		[Required]
        [MaxLength(200, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Event Name")]
		public string EventName { get; set; }

		[Required]
		[QuarterIncrement]
		[Range(0, 100)]
		[Display(Name = "Event Hours")]
		public double? EventHours { get; set; }

		[Required]
		[WholeNumber]
		[Range(0, 999999)]
		[Display(Name = "Number of People Reached")]
		public int? NumPeopleReached { get; set; }

		[Required]
		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Event Date")]
		public DateTime? EventDate { get; set; }

        [MaxLength(250, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Comments")]
		public string Comment { get; set; }

		[DataType(DataType.Text)]
        [MaxLength(90, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string Location { get; set; }

		[Display(Name = "County")]
		public int? CountyID { get; set; }

		[Display(Name = "State")]
		public int? StateID { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual IList<EventDetailStaff> EventDetailStaff { get; set; }

		public int saveAddNew { get; set; }

		public string ReturnURL { get; set; }
	}
}