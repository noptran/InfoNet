using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Mapping.Centers {
	public class SystemMessageMap : EntityTypeConfiguration<SystemMessage> {
		public SystemMessageMap() {
			HasKey(s => s.Id);

			Property(t => t.ProviderIdsString).HasMaxLength(25);
			Property(t => t.CenterIdsString).HasMaxLength(4000);
			Property(t => t.LocationIdsString).HasMaxLength(4000);
			Property(t => t.Title).HasMaxLength(250);
			Property(t => t.Message).HasMaxLength(500);
			Property(t => t.Details).HasMaxLength(500);
			Property(t => t.LinkUrl).HasMaxLength(250);
			Property(t => t.LinkText).HasMaxLength(25);

			ToTable("T_SystemMessages");
			Property(s => s.Id).HasColumnName("ID");
			Property(s => s.ProviderIdsString).HasColumnName("ProviderIDs");
			Property(s => s.CenterIdsString).HasColumnName("CenterIDs");
			Property(s => s.LocationIdsString).HasColumnName("LocationIDs");
			Property(s => s.ModeId).HasColumnName("ModeID");
			Property(s => s.Title).HasColumnName("Title");
			Property(s => s.Message).HasColumnName("Message");
			Property(s => s.Details).HasColumnName("Details");
			Property(s => s.LinkUrl).HasColumnName("LinkURL");
			Property(s => s.LinkText).HasColumnName("LinkText");
			Property(s => s.IsDownload).HasColumnName("IsDownload");
			Property(s => s.IsHot).HasColumnName("IsHot");
			Property(s => s.PostedDate).HasColumnName("PostedDate");
			Property(s => s.ExpirationDate).HasColumnName("ExpirationDate");
			Property(s => s.RevisionStamp).HasColumnName("RevisionStamp");
		}
	}
}