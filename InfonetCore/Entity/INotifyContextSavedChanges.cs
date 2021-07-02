using System.Data.Entity;

namespace Infonet.Core.Entity {
	/**
	 * Interface used by EnhancedDbContext to notify its Entities that all
	 * changes have been saved successfully. (i.e. context.SaveChanges(); )
	**/
	public interface INotifyContextSavedChanges {
		void OnContextSavedChanges(EntityState prior);
	}
}