using System;
using System.Linq;
using System.Reflection;

namespace Infonet.Core.Reflection {
	public static class AssemblyExtensions {
		public static TAttribute GetCustomAttribute<TAttribute>(this Assembly self) where TAttribute : Attribute {
			return self.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>().SingleOrDefault();
		}

		public static string GetCustomAttributeValue<TAttribute>(this Assembly self, Func<TAttribute, string> selector) where TAttribute : Attribute {
			return self.GetCustomAttribute<TAttribute>().NotNull(selector);
		}
	}
}