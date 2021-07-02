// Pruned a cleansed version of Microsoft class TypeHelpers used in MVC binding.

using System;
using System.Linq;

namespace Infonet.Web.Mvc.Binding.Microsoft {
	internal static class TypeHelpers {
		public static Type ExtractGenericInterface(Type queryType, Type interfaceType) {
			if (MatchesGenericType(queryType, interfaceType))
				return queryType;

			return MatchGenericTypeFirstOrDefault(queryType.GetInterfaces(), interfaceType);
		}

		private static bool IsNullableValueType(Type type) {
			return Nullable.GetUnderlyingType(type) != null;
		}

		private static bool MatchesGenericType(Type type, Type matchType) {
			return type.IsGenericType && type.GetGenericTypeDefinition() == matchType;
		}

		private static Type MatchGenericTypeFirstOrDefault(Type[] types, Type matchType) {
			return types.FirstOrDefault(type => MatchesGenericType(type, matchType));
		}

		public static bool TypeAllowsNullValue(Type type) {
			return !type.IsValueType || IsNullableValueType(type);
		}
	}
}