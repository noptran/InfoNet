using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class BetweenNineteenThirtyTodayAttributeAdapter : DataAnnotationsModelValidator<BetweenNineteenThirtyTodayAttribute> {
		public BetweenNineteenThirtyTodayAttributeAdapter(ModelMetadata metadata, ControllerContext context, BetweenNineteenThirtyTodayAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] {
				new ModelClientValidationRule {
					ErrorMessage = "The field " + Metadata.DisplayName + " must be between 1/1/1930 and today.",
					ValidationType = "betweennineteenthirtytoday"
				}
			};
		}
	}
}