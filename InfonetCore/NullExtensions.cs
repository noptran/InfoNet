using System;

namespace Infonet.Core {
	public static class NullExtensions {
		public static TResult NotNull<TSource, TResult>(this TSource source, Func<TSource, TResult> function) {
			return source == null ? default(TResult) : function(source);
		}
	}
}