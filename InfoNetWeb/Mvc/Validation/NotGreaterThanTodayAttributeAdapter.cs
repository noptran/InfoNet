using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class NotGreaterThanTodayAttributeAdapter : DataAnnotationsModelValidator<NotGreaterThanTodayAttribute> {
		public NotGreaterThanTodayAttributeAdapter(ModelMetadata metadata, ControllerContext context, NotGreaterThanTodayAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			return new[] {
				new ModelClientValidationRule {
					ErrorMessage = "The field " + Metadata.DisplayName + " cannot be greater than today.",
					ValidationType = "notgreaterthantoday"
				}
			};
		}
	}
}