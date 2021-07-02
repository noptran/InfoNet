using System;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Data.Importing {
	/** Intended for use by ServicesImport when throwing exceptions deemed safe for end-user display. **/
	public class ImportException : Exception {
		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public ImportException(string message) : base(message) { }

		public ImportException(string message, Exception innerException) : base(message, innerException) { }
	}
}