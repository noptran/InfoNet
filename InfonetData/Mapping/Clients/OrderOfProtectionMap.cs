using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class OrderOfProtectionMap : EntityTypeConfiguration<OrderOfProtection> {
		public OrderOfProtectionMap() {
			// Primary Key
			HasKey(t => t.OP_ID);

			// Properties
			Property(t => t.Comments)
				.HasMaxLength(300);

			// Table & Column Mappings
			ToTable("Ts_OrderOfProtection");
			Property(t => t.OP_ID).HasColumnName("OP_ID");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.StatusID).HasColumnName("StatusID");
			Property(t => t.DateFiled).HasColumnName("DateFiled");
			Property(t => t.CountyID).HasColumnName("CountyID");
			Property(t => t.DateIssued).HasColumnName("DateIssued");
			Property(t => t.DateVacated).HasColumnName("DateVacated");
			Property(t => t.OriginalExpirationDate).HasColumnName("OriginalExpirationDate");
			Property(t => t.TypeOfOPID).HasColumnName("TypeOfOPID");
			Property(t => t.ForumID).HasColumnName("ForumID");
			Property(t => t.Comments).HasColumnName("Comments");
			Property(t => t.LocationID).HasColumnName("LocationID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.CivilNoContactOrderId).HasColumnName("CivilNoContactOrderID");
			Property(t => t.CivilNoContactOrderTypeId).HasColumnName("CivilNoContactOrderTypeID");
			Property(t => t.CivilNoContactOrderRequestId).HasColumnName("CivilNoContactOrderRequestID");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.OrdersOfProtection)
				.HasForeignKey(d => new { d.ClientId, d.CaseId });
		}
	}
}