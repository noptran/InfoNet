using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Web.Models.Identity;

namespace Infonet.Web.ViewModels.Account {
	public class ManageAccountViewModel {
		public ManageAccountViewModel() {
			Roles = new List<ApplicationRole>();
		}

		[Required]
        [MinLength(6, ErrorMessageResourceName = "StringMinLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string Username { get; set; }

		[Display(Name = "Center Name")]
		public string CenterName { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		public List<ApplicationRole> Roles { get; set; }
	}

	public class ChangePasswordViewModel : ConfirmNewPassword {
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }
	}
}