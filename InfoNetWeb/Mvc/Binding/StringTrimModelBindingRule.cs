using System.Web.Mvc;
using Infonet.Core.Entity.Binding;

namespace Infonet.Web.Mvc.Binding {
	/// <summary>
	///     MVC model binder which trims string values decorated with the <see cref="StringTrimAttribute" />.
	/// </summary>
	public class StringTrimModelBindingRule : IModelBindingRule {
		public bool AppliesTo(ModelBindingContext bindingContext) {
			var propertyInfo = bindingContext.ModelMetadata.ContainerType?.GetProperty(bindingContext.ModelMetadata.PropertyName);
			return propertyInfo != null && propertyInfo.GetCustomAttributes(typeof(StringTrimAttribute), true).Length > 0;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
			var originalValueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (originalValueResult == null)
				return null;

			string boundValue = originalValueResult.AttemptedValue;

			if (!string.IsNullOrEmpty(boundValue))
				boundValue = boundValue.Trim();

			// Register updated "attempted" value with the model state
			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, new ValueProviderResult(originalValueResult.RawValue, boundValue, originalValueResult.Culture));

			return boundValue;
		}
	}
}