using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Data.Looking {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
	public interface ILookupSelection : IEnumerable<LookupCode> {
		bool IsEmpty { get; }

		ILookupSelection Exclude(params int[] codeIds);

		ILookupSelection Include(params int[] codeIds);

		ILookupSelection ExcludeFor(Provider provider, params int[] codeIds);

		ILookupSelection IncludeFor(Provider provider, params int[] codeIds);
	}
}