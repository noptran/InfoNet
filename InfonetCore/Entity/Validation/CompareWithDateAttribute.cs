using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Reflection;

namespace Infonet.Core.Entity.Validation {
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CompareWithDateAttribute : CompareAttribute {
		public CompareType ComparisonType { get; }

		public CompareWithDateAttribute(string otherProperty, CompareType comparisonType) : base(otherProperty) {
			ComparisonType = comparisonType;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			var propertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);

			string otherPropertyDisplayName = GetOtherPropertyDisplayName(validationContext.ObjectType);
			var otherPropertyValue = propertyInfo.GetValue(validationContext.ObjectInstance);

			var firstDate = value as DateTime?;
			var secondDate = otherPropertyValue as DateTime?;

			if (!firstDate.HasValue || !secondDate.HasValue)
				return ValidationResult.Success;

			switch (ComparisonType) {
				case CompareType.GreaterThan:
					if (firstDate <= secondDate)
						return new ValidationResult($"{validationContext.DisplayName} must be later than {otherPropertyDisplayName}.", new[] { validationContext.MemberName, OtherProperty });
					break;
				case CompareType.GreaterThanEqualTo:
					if (firstDate < secondDate)
						return new ValidationResult($"{validationContext.DisplayName} must be later than {otherPropertyDisplayName}.", new[] { validationContext.MemberName, OtherProperty });
					break;
				case CompareType.LessThan:
					if (firstDate >= secondDate)
						return new ValidationResult($"{validationContext.DisplayName} must be earlier than {otherPropertyDisplayName}.", new[] { validationContext.MemberName, OtherProperty });
					break;
				case CompareType.LessThanEqualTo:
					if (firstDate > secondDate)
						return new ValidationResult($"{validationContext.DisplayName} must be earlier than {otherPropertyDisplayName}.", new[] { validationContext.MemberName, OtherProperty });
					break;
				default:
					throw new NotImplementedException("Comparison type not yet implemented for the CompareWithDate attribute.");
			}

			return ValidationResult.Success;
		}

		public string GetOtherPropertyDisplayName(Type context) {
			return context.GetProperty(OtherProperty).GetDisplayName();
		}
	}

	public enum CompareType {
		GreaterThan,
		GreaterThanEqualTo,
		LessThan,
		LessThanEqualTo
	}
}