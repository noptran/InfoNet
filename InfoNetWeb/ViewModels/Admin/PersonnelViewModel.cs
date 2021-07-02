using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Admin {
	public class PersonnelViewModel {
		public int saveAddNew { get; set; }

		public string ReturnURL { get; set; }

		public int SvId { get; set; }

		public int CenterId { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Lookup("Sex")]
		[Display(Name = "Gender Identity")]
		public int? SexId { get; set; }

		[Lookup("Race")]
		[Display(Name = "Race/Ethnicity")]
		public int? RaceId { get; set; }

		[Display(Name = "Personnel Type")]
		public int? PersonnelTypeId { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string Title { get; set; }

		[Display(Name = "Student")]
		public bool CollegeUnivStudent { get; set; }

		public string Department { get; set; }

		[Display(Name = "Work Phone")]
		[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone number. Must be a 10 digit number.")]
		public string WorkPhone { get; set; }

		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }

		[Required]
		[BetweenNineteenSeventyToday]
		[Display(Name = "Start Date")]
		[DataType(DataType.DateTime)]
		public DateTime? StartDate { get; set; }

		[BetweenNineteenSeventyToday]
		[Display(Name = "Termination Date")]
		[DataType(DataType.DateTime)]
		public DateTime? TerminationDate { get; set; }

		[Display(Name = "Supervisor")]
		public int? SupervisorId { get; set; }

		public string Type {
			get { return TypeId == 1 ? "S" : "V"; }
		}

		public int TypeId { get; set; }
	}
}