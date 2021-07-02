using System;
using System.Collections.Generic;
using System.IO;
using Infonet.Core.Collections;

namespace Infonet.Core.IO {
	public static class TextWriterExtensions {
		public static void WriteConjoined<TItem>(this TextWriter w, string conjunction, string itemFormatOrNull, IEnumerable<TItem> items) {
			WriteConjoined(w, ',', conjunction, itemFormatOrNull, items);
		}

		public static void WriteConjoined<TItem>(this TextWriter w, char comma, string conjunction, string itemFormatOrNull, IEnumerable<TItem> items) {
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			var en = new LookaheadEnumerator<TItem>(items);
			bool wasPreviousFirst = false;
			while (en.MoveNext()) {
				if (!en.IsFirst) {
					if (!wasPreviousFirst || !en.IsLast)
						w.Write(comma);
					if (en.IsLast) {
						w.Write(' ');
						w.Write(conjunction);
					}
					w.Write(' ');
				}
				w.WriteFormatOptional(en.Current, itemFormatOrNull);
				wasPreviousFirst = en.IsFirst;
			}
		}

		private static void WriteFormatOptional<TItem>(this TextWriter w, TItem item, string formatOrNull) {
			if (formatOrNull == null)
				w.Write(item);
			else
				w.Write(formatOrNull, item);
		}

		public static string ToConjoinedString<TItem>(this IEnumerable<TItem> self, string conjunction, string itemFormat = null) {
			using (var sw = new StringWriter()) {
				sw.WriteConjoined(conjunction, itemFormat, self);
				return sw.ToString();
			}
		}
	}
}