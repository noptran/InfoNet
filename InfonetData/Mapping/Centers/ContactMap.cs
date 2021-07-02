using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Mapping.Centers {
	public class ContactMap : EntityTypeConfiguration<Contact> {
		public ContactMap() {
			// Primary Key
			HasKey(t => t.ContactId);

			// Properties
			Property(t => t.ContactName)
				.HasMaxLength(50);

			// Table & Column Mappings
			ToTable("T_Contact");
			Property(t => t.ContactId).HasColumnName("ContactID");
			Property(t => t.ContactName).HasColumnName("ContactName");
			Property(t => t.CenterId).HasColumnName("CenterID");
			Property(t => t.Active).HasColumnName("Active");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasOptional(t => t.Center)
				.WithMany(t => t.Contacts)
				.HasForeignKey(d => d.CenterId);
		}
	}
}