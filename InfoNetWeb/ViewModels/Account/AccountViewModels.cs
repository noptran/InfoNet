using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.ViewModels.Account {
	public class LoginViewModel {
		[Required]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }
	}

	public class RegisterViewModel {
		public RegisterViewModel() {
			Roles = new string[0];
		}

		[Required]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Center")]
		public int? CenterId { get; set; }

		[Required]
		[MaxLength(100, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		[MinLength(8, ErrorMessageResourceName = "StringMinLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessageResourceName = "StringConfirmPasswordMessage", ErrorMessageResourceType = typeof(Resource))]
		public string ConfirmPassword { get; set; }

		public string[] Roles { get; set; }
	}

	public class ResetPasswordViewModel {
		[Required]
		[Display(Name = "Username")]
		public string Username { get; set; }

		[Required]
		[MaxLength(100, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		[MinLength(8, ErrorMessageResourceName = "StringMinLengthMessage", ErrorMessageResourceType = typeof(Resource))]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm New Password")]
		[Compare("Password", ErrorMessageResourceName = "StringConfirmNewPasswordMessage", ErrorMessageResourceType = typeof(Resource))]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
	}

	public class ForgotPasswordViewModel {
		[Required]
		[Display(Name = "Username")]
		public string Username { get; set; }
	}
}