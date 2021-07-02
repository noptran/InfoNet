using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class BetweenNineteenSeventyTodayAttributeAdapter : DataAnnotationsModelValidator<BetweenNineteenSeventyTodayAttribute> {
		public BetweenNineteenSeventyTodayAttributeAdapter(ModelMetadata metadata, ControllerContext context, BetweenNineteenSeventyTodayAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] {
				new ModelClientValidationRule {
					ErrorMessage = "The field " + Metadata.DisplayName + " must be between 1/1/1970 and today.",
					ValidationType = "betweennineteenseventytoday"
				}
			};
		}
	}
}