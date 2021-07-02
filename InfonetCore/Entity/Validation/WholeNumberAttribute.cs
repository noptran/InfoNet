using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Core.Entity.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public class WholeNumberAttribute : ValidationAttribute {
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			if (value == null)
				return ValidationResult.Success;

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (Convert.ToDouble(value) % 1 != 0)
				return new ValidationResult($"The field {validationContext.DisplayName} must be a whole number.");

			return ValidationResult.Success;
		}
	}
}