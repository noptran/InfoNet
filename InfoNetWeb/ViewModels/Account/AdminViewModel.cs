using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Web.Models.Identity;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Account {
	public class RoleSearchViewModel : PagedListPagination {
		public RoleSearchViewModel() {
			PageSize = 50;
		}

		public IPagedList<ApplicationRole> RoleList { get; set; }
	}

	public class RoleViewModel {
		public int Id { get; set; }

		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Role Name")]
		public string Name { get; set; }

		public string Description { get; set; }

		public IEnumerable<ApplicationUser> Users { get; set; }
	}

	public class UserSearchViewModel : PagedListPagination {
		public UserSearchViewModel() {
			PageSize = 50;
		}

		public IPagedList<ApplicationUser> UserList { get; set; }
	}

	public class EditUserViewModel {
		public EditUserViewModel() {
			Roles = new string[0];
		}

		public int Id { get; set; }

		[Required(AllowEmptyStrings = false)]
		[Display(Name = "Email")]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required]
		[Display(Name = "Center")]
		public int CenterId { get; set; }

		public string[] Roles { get; set; }
	}

	public class PasswordResetViewModel {
		public int Id { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm New Password")]
		public string ConfirmPassword { get; set; }
	}

	public class ConfirmNewPassword {
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		[MaxLength(100, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		[MinLength(8, ErrorMessageResourceName = "StringMinLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm New Password")]
		[Compare("NewPassword", ErrorMessageResourceName = "StringConfirmNewPasswordMessage", ErrorMessageResourceType = typeof(Resource))]
		public string ConfirmPassword { get; set; }
	}
}