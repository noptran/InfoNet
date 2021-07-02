using System.Collections.Generic;
using System.Web.Mvc;

namespace Infonet.Web.Mvc.Validation {
	public class FileExtensionAttributeAdapter : DataAnnotationsModelValidator<FileExtensionAttribute> {
		public FileExtensionAttributeAdapter(ModelMetadata metadata, ControllerContext context, FileExtensionAttribute attribute) : base(metadata, context, attribute) { }

		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
			var rule = new ModelClientValidationRule {
				ErrorMessage = string.Format(Attribute.ErrorMessageTemplate, Metadata.DisplayName),
				ValidationType = "fileextension"
			};
			rule.ValidationParameters.Add("allowedextensions", string.Join(",", Attribute.AllowedExtensions));
			return new[] { rule };
		}
	}
}