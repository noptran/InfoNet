using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Infonet.Data.Looking {
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class LookupAttribute : ValidationAttribute {
		private readonly string _name;
		private readonly PropertyInfo _property;

		public LookupAttribute(string name) {
			_name = name;
			_property = typeof(Lookups).GetProperty(_name);
		}

		public Lookup Lookup {
			get { return (Lookup)_property.GetValue(null); }
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			if (value != null && !((Lookup)_property.GetValue(null)).Contains(Convert.ToInt32(value)))
				return new ValidationResult("The field " + validationContext.DisplayName + " must be a valid " + _name + " code.");
			return ValidationResult.Success;
		}
	}
}