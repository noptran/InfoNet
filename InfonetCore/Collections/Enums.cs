using System;
using System.Collections.Generic;
using System.Linq;

namespace Infonet.Core.Collections {
	public static class Enums {
		public static TEnum Parse<TEnum>(string enumName) {
			var enumType = typeof(TEnum);
			if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
				enumType = enumType.GetGenericArguments()[0];
			return (TEnum)Enum.Parse(enumType, enumName);
		}

		public static IEnumerable<TEnum> GetValues<TEnum>() {
			return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
		}
	}
}