using System;
using System.Diagnostics.CodeAnalysis;
using Infonet.Core.Collections;
using Newtonsoft.Json.Linq;

namespace Infonet.Web.Mvc.Collections {
	public static class JTokenExtensions {
		[SuppressMessage("ReSharper", "ExpressionIsAlwaysNull")]
		public static TEnum Enum<TEnum>(this JToken token) {
			if (token.Type == JTokenType.Integer)
				return (TEnum)(object)token.Value<int>();
			if (token.Type == JTokenType.String)
				return Enums.Parse<TEnum>(token.Value<string>());
			if (token.Type == JTokenType.Null) {
				var result = default(TEnum);
				if (result != null)
					throw new NotSupportedException("JTokenType.Null cannot be expressed as non-nullable type " + typeof(TEnum));
				return result;
			}
			throw new NotSupportedException("JTokenType " + token.Type + " not convertible to enum");
		}
	}
}