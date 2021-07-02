using System;

namespace Infonet.Core.Entity {
	/** Interface used by EnhancedDbContext to refresh RevisionStamps on added/modified
	 *  entities after validating but just prior to saving them.  A correct implementation of
	 *  RevisionStamp will not allow any entity to become invalid upon setting RevisionStamp
	 *  to a non-null value. **/
	public interface IRevisable {
		// ReSharper disable once UnusedMemberInSuper.Global
		DateTime? RevisionStamp { get; set; }
	}
}