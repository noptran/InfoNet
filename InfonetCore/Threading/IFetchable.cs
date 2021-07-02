namespace Infonet.Core.Threading {
	public interface IFetchable<in TElement> {
		/**
		 * Fills as much of buffer as possible starting at index 0.  Returns the number of filled.
		 * Returns -1 if nothing more to fetch ever.
		 **/
		// ReSharper disable once UnusedMemberInSuper.Global
		int Fetch(TElement[] buffer);

		/**
		 * Starting at offset, fills buffer with as many as count.  Returns the number of filled.
		 * Returns -1 if nothing more to fetch ever.
		 **/
		int Fetch(TElement[] buffer, int offset, int count);
	}
}