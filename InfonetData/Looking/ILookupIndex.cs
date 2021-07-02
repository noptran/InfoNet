using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Data.Looking {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
	public interface ILookupIndex : IEnumerable<LookupCode> {
		bool IsEmpty { get; }

		/** Returns null when codeId is null. **/
		LookupCode this[int? codeId] { get; }

		/** Throws IndexOutOfRangeException when codeId is not found. **/
		LookupCode this[int codeId] { get; }
	}
}