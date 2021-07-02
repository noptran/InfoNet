using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Core.Entity.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class NotGreaterThanTodayAttribute : ValidationAttribute {
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			if (value == null)
				return ValidationResult.Success;

			if (Convert.ToDateTime(value) > DateTime.Now)
				return new ValidationResult("The field " + validationContext.DisplayName + " cannot be greater than today.");

			return ValidationResult.Success;
		}
	}
}