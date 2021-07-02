using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Reflection;
using Infonet.Web.Mvc.Binding.Microsoft;

namespace Infonet.Web.Mvc.Binding {
	public class ModelBindingRuleSet : IModelBinder {
		private readonly IModelBinder _default;
		private readonly IModelBindingRule[] _ruleSequence;

		public ModelBindingRuleSet(IModelBinder defaultTo, params IModelBindingRule[] ruleSequence) {
			_default = defaultTo;
			_ruleSequence = ruleSequence;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			try {
				foreach (var each in _ruleSequence)
					if (each.AppliesTo(bindingContext))
						return each.BindModel(controllerContext, bindingContext);

				return _default.BindModel(controllerContext, bindingContext);
			} finally {
				var modelState = bindingContext.ModelState[bindingContext.ModelName];
				if (modelState != null)
					OnBindExceptions(modelState, bindingContext.ModelMetadata.ContainerType, bindingContext.ModelMetadata.PropertyName, bindingContext.ModelMetadata.GetDisplayName());
			}
		}

		private static void OnBindExceptions(ModelState modelState, Type containerType, string propertyName, string displayName) {
			if (containerType == null)
				return;

			var exceptionsWithoutMessages = modelState.Errors.Where(err => string.IsNullOrEmpty(err.ErrorMessage) && err.Exception != null).ToList();
			if (exceptionsWithoutMessages.Count == 0)
				return;

			var propertyDescriptor = TypeDescriptorHelper.Get(containerType).GetProperties().Find(propertyName, false);
			if (propertyDescriptor == null)
				return;

			var attributes = propertyDescriptor.Attributes.Filter<OnBindExceptionAttribute>().ToArray();
			foreach (var eachError in exceptionsWithoutMessages)
				foreach (var eachAttribute in attributes)
					if (eachAttribute.AppliesTo(eachError.Exception, true)) {
						string newMessage = string.Format(CultureInfo.CurrentCulture, eachAttribute.MessageTemplate, modelState.Value.AttemptedValue, displayName);
						modelState.Errors.Remove(eachError);
						modelState.Errors.Add(newMessage);
						break;
					}
		}
	}
}