using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class WholeNumberAttributeAdapter : DataAnnotationsModelValidator<WholeNumberAttribute> {
		public WholeNumberAttributeAdapter(ModelMetadata metadata, ControllerContext context, WholeNumberAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] {
				new ModelClientValidationRule {
					ErrorMessage = "The field " + Metadata.DisplayName + " must be a whole number.",
					ValidationType = "wholenumber"
				}
			};
		}
	}
}