using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Core.Entity.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class BetweenJulyTwoThousandEightTodayAttribute : ValidationAttribute {
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			if (value == null)
				return ValidationResult.Success;

			var date = Convert.ToDateTime(value);
			if (date > DateTime.Now || date < DateTime.Parse("07/01/2008"))
				return new ValidationResult("The field " + validationContext.DisplayName + " must be between 07/01/2008 and today.");

			return ValidationResult.Success;
		}
	}
}