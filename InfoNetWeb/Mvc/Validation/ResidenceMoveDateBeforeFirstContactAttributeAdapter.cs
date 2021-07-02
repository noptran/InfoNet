using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Data.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
	public class ResidenceMoveDateBeforeFirstContactAttributeAdapter : DataAnnotationsModelValidator<ResidenceMoveDateBeforeFirstContactAttribute> {
		public ResidenceMoveDateBeforeFirstContactAttributeAdapter(ModelMetadata metadata, ControllerContext context, ResidenceMoveDateBeforeFirstContactAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			yield return new ModelClientValidationRule {
				ErrorMessage = Attribute.FormatErrorMessage(Metadata.GetDisplayName()),
				ValidationType = "reasonable"
			};
		}
	}
}