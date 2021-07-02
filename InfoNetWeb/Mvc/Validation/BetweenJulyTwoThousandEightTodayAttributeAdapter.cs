using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class BetweenJulyTwoThousandEightTodayAttributeAdapter : DataAnnotationsModelValidator<BetweenJulyTwoThousandEightTodayAttribute> {
		public BetweenJulyTwoThousandEightTodayAttributeAdapter(ModelMetadata metadata, ControllerContext context, BetweenJulyTwoThousandEightTodayAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] {
				new ModelClientValidationRule {
					ErrorMessage = "The field " + Metadata.DisplayName + " must be between 07/01/2008 and today.",
					ValidationType = "betweenjulytwothousandeighttoday"
				}
			};
		}
	}
}