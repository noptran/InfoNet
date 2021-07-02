using System.Diagnostics.CodeAnalysis;

namespace Infonet.Core.Collections {
	[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
	public interface IKeyAccess {
		/* Like IDictionary<> without specific return type. */
		object this[Key key] { get; }

		/* Like IDictionary<> without specific value parameter type. */
		void Add(Key key, object value);

		/* Required by IDictionary<>. */
		bool ContainsKey(Key key);

		/* Required by IDictionary<>. */
		bool Remove(Key key);
	}
}