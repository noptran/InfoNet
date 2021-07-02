namespace Infonet.Data.Models.Looking {
	public class LookupListItemAssignment {
		public int? Id { get; set; }
		public int TableId { get; set; }
		public int ProviderId { get; set; }
		public int? CodeId { get; set; }
		public int? DisplayOrder { get; set; }
		public bool IsActive { get; set; }
		public virtual LookupListTable Table { get; set; }
	}
}