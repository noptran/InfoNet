using System.Collections.Generic;

namespace Infonet.Data.Models.Looking {
	public class LookupListTable {
		public LookupListTable() {
			ItemAssignments = new List<LookupListItemAssignment>();
		}

		public int TableId { get; set; }
		public string TableName { get; set; }
		public string Description { get; set; }
		public string DisplayName { get; set; }
		public virtual ICollection<LookupListItemAssignment> ItemAssignments { get; set; }
	}
}