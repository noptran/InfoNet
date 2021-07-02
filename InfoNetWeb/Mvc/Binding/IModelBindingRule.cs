using System.Web.Mvc;

namespace Infonet.Web.Mvc.Binding {
	public interface IModelBindingRule : IModelBinder {
		bool AppliesTo(ModelBindingContext bindingContext);
	}
}