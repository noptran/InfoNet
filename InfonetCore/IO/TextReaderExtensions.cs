using System;
using System.IO;

namespace Infonet.Core.IO {
	public static class TextReaderExtensions {
		// ReSharper disable once MemberCanBePrivate.Global
		public const int DEFAULT_COPY_BUFFER_SIZE = 4096;

		public static long CopyTo(this TextReader source, TextWriter target, int bufferSize = DEFAULT_COPY_BUFFER_SIZE) {
			if (bufferSize < 1)
				throw new ArgumentOutOfRangeException(nameof(bufferSize));

			long result = 0;
			int count;
			var buffer = new char[bufferSize];
			while ((count = source.Read(buffer, 0, buffer.Length)) != 0) {
				target.Write(buffer, 0, count);
				result += count;
			}
			return result;
		}
	}
}