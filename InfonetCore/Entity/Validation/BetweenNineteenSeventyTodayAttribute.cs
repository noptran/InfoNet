﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Core.Entity.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class BetweenNineteenSeventyTodayAttribute : ValidationAttribute {
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			if (value == null)
				return ValidationResult.Success;

			var date = Convert.ToDateTime(value);
			if (date > DateTime.Now || date < DateTime.Parse("01/01/1970"))
				return new ValidationResult("The field " + validationContext.DisplayName + " must be between 1/1/1970 and today.");

			return ValidationResult.Success;
		}
	}
}