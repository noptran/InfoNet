using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class QuarterIncrementAttributeAdapter : DataAnnotationsModelValidator<QuarterIncrementAttribute> {
		public QuarterIncrementAttributeAdapter(ModelMetadata metadata, ControllerContext context, QuarterIncrementAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] {
				new ModelClientValidationRule {
					ErrorMessage = "The field " + Metadata.DisplayName + " must be divisable by 0.25.",
					ValidationType = "quarterincrement"
				}
			};
		}
	}
}