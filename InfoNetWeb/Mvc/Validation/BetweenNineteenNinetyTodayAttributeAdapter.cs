using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class BetweenNineteenNinetyTodayAttributeAdapter : DataAnnotationsModelValidator<BetweenNineteenNinetyTodayAttribute> {
		public BetweenNineteenNinetyTodayAttributeAdapter(ModelMetadata metadata, ControllerContext context, BetweenNineteenNinetyTodayAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] {
				new ModelClientValidationRule {
					ErrorMessage = "The field " + Metadata.DisplayName + " must be between 1/1/1990 and today.",
					ValidationType = "betweennineteenninetytoday"
				}
			};
		}
	}
}