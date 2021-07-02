using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Core.Entity.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class NotLessThanNineteenSeventyAttribute : ValidationAttribute {
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			if (value == null)
				return ValidationResult.Success;

			if (Convert.ToDateTime(value) < DateTime.Parse("01/01/1970"))
				return new ValidationResult("The field " + validationContext.DisplayName + " must not be less than 1/1/1970.");

			return ValidationResult.Success;
		}
	}
}