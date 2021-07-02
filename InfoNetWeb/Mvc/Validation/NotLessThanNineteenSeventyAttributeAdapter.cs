using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class NotLessThanNineteenSeventyAttributeAdapter : DataAnnotationsModelValidator<NotLessThanNineteenSeventyAttribute> {
		public NotLessThanNineteenSeventyAttributeAdapter(ModelMetadata metadata, ControllerContext context, NotLessThanNineteenSeventyAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] {
				new ModelClientValidationRule {
					ErrorMessage = "The field " + Metadata.DisplayName + " must not be less than 1/1/1970.",
					ValidationType = "notlessthannineteenseventy"
				}
			};
		}
	}
}