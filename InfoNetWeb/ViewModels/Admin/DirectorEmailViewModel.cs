using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.ViewModels.Admin {
    public class DirectorEmailViewModel {
        public int CenterId { get; set; }

        [Display(Name = "Center Name")]
        public string CenterName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Director Email Address")]
        public string DirectorEmail { get; set; }
        
        public bool ShouldEdit { get; set; }
    }
}