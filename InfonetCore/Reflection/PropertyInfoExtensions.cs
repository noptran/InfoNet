using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Infonet.Core.Reflection {
	public static class PropertyInfoExtensions {
		public static string GetDisplayName(this PropertyInfo propInfo) {
			return propInfo.GetAttribute<DisplayAttribute>()?.Name ?? propInfo.Name;
		}
	}
}