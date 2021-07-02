using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Core.Collections {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class Parameters : Dictionary<string, object> {
		public Parameters() { }

		public Parameters(IDictionary<string, object> dictionary) : base(dictionary) { }
	}
}