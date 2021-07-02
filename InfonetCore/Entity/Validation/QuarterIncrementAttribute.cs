using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Core.Entity.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public class QuarterIncrementAttribute : ValidationAttribute {
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			if (value == null)
				return ValidationResult.Success;

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (Convert.ToDouble(value) % 0.25 != 0)
				return new ValidationResult("The field " + validationContext.DisplayName + " must be divisable by 0.25.");

			return ValidationResult.Success;
		}
	}
}