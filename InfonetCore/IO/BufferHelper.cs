using System;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Core.IO {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class BufferHelper {
		public const int DEFAULT_STREAMWRITER_BUFFER_SIZE = 1024;
		public const int DEFAULT_STREAMREADER_BUFFER_SIZE = 1024;
		public const int DEFAULT_FILESTREAM_BUFFER_SIZE = 4096;
		public const int DEFAULT_STREAM_COPY_BUFFER_SIZE = 81920;

		public static void AssertBufferIsValid<T>(T[] buffer, int offset, int count) {
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));
			if (offset < 0 || offset >= buffer.Length)
				throw new ArgumentOutOfRangeException(nameof(offset));
			if (count < 0 || count > buffer.Length - offset)
				throw new ArgumentOutOfRangeException(nameof(count));
		}
	}
}