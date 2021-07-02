using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Core.Entity.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class LogicalBooleanAttribute : ValidationAttribute {
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			try {
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				Convert.ToBoolean(value);
				return ValidationResult.Success;
			} catch {
				return new ValidationResult("The field " + validationContext.DisplayName + " only accepts true or false.");
			}
		}
	}
}