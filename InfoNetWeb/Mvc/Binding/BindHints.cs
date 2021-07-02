using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Infonet.Core.Entity.Binding;

namespace Infonet.Web.Mvc.Binding {
	public static class BindHints {
		public static void GenerateBindAttributes() {
			string bindHintAssembly = typeof(BindHintAttribute).Assembly.GetName().Name;
			foreach (var eachAssembly in AppDomain.CurrentDomain.GetAssemblies())
				if (eachAssembly.GetName().Name == bindHintAssembly || eachAssembly.GetReferencedAssemblies().Any(a => a.Name == bindHintAssembly))
					foreach (var eachType in eachAssembly.GetTypes())
						foreach (BindHintAttribute eachHint in eachType.GetCustomAttributes(typeof(BindHintAttribute), false))
							TypeDescriptor.AddAttributes(eachType, new BindAttribute { Include = eachHint.Include, Exclude = eachHint.Exclude, Prefix = eachHint.Prefix });
		}
	}
}