using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Infonet.Core.Collections {
	public static class EnumExtensions {
		public static string GetDisplayName(this Enum enumValue) {
			string enumString = enumValue.ToString();
			return enumValue.GetType().GetMember(enumString).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? enumString;
		}

		public static string GetShortName(this Enum enumValue) {
			string enumString = enumValue.ToString();
			return enumValue.GetType().GetMember(enumString).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.GetShortName() ?? enumString;
		}

		public static int GetOrder(this Enum enumValue) {
			return enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.Order ?? 0;
		}

		public static int ToInt32(this Enum enumValue) {
			return Convert.ToInt32(enumValue);
		}
	}
}