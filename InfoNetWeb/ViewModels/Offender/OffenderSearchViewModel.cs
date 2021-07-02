using System.ComponentModel.DataAnnotations;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Offender {
	public class OffenderSearchViewModel {
        [MaxLength(20, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Offender ID")]
		public string OffenderCode { get; set; }

		[Lookup("GenderIdentity")]
		[Display(Name = "Gender Identity")]
		public int? SexId { get; set; }

		[Lookup("Race")]
		[Display(Name = "Race/Ethnicity")]
		public int? RaceId { get; set; }

		public int? AgeFrom { get; set; }

		public int? AgeTo { get; set; }
	}
}