using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.Mvc.Validation {
    public class BetweenNineteenFiftyTodayAttributeAdapter : DataAnnotationsModelValidator<BetweenNineteenFiftyTodayAttribute> {
        public BetweenNineteenFiftyTodayAttributeAdapter(ModelMetadata metadata, ControllerContext context, BetweenNineteenFiftyTodayAttribute attribute) : base(metadata, context, attribute) { }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules() {
            return new[] {
                new ModelClientValidationRule {
                    ErrorMessage = "The field " + Metadata.DisplayName + " must be between 1/1/1950 and today.",
                    ValidationType = "betweennineteenfiftytoday"
                }
            };
        }
    }
}