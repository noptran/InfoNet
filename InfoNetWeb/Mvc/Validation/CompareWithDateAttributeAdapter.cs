using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class CompareWithDateAttributeAdapter : DataAnnotationsModelValidator<CompareWithDateAttribute> {
		public CompareWithDateAttributeAdapter(ModelMetadata metadata, ControllerContext context, CompareWithDateAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			string otherFieldDisplayName = Attribute.GetOtherPropertyDisplayName(Metadata.ContainerType);
			string message = "";

			switch (Attribute.ComparisonType) {
				case CompareType.GreaterThan:
				case CompareType.GreaterThanEqualTo:
					message = $"{Metadata.DisplayName} must be later than {otherFieldDisplayName}.";
					break;
				case CompareType.LessThan:
				case CompareType.LessThanEqualTo:
					message = $"{Metadata.DisplayName} must be earlier than {otherFieldDisplayName}.";
					break;
			}

			var validationRule = new ModelClientValidationRule {
				ErrorMessage = message,
				ValidationType = "comparewithdate"
			};

			validationRule.ValidationParameters.Add("propertyname", Metadata.PropertyName);
			validationRule.ValidationParameters.Add("otherpropertyname", Attribute.OtherProperty);
			validationRule.ValidationParameters.Add("comparisontype", Attribute.ComparisonType.ToString());

			return new[] { validationRule };
		}
	}
}