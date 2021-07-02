using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class DCFSAllegationRespondentMap : EntityTypeConfiguration<DCFSAllegationRespondent> {
		public DCFSAllegationRespondentMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			// Table & Column Mappings
			ToTable("TS_DCFSAllegationsRespondents");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.DCFSAllegations_FK).HasColumnName("DCFSAllegations_FK");
			Property(t => t.RespondentId).HasColumnName("RespondentID");
			Property(t => t.RespondentType).HasColumnName("RespondentType");

			// Relationships
			HasRequired(t => t.Allegation)
				.WithMany(t => t.Respondents)
				.HasForeignKey(d => d.DCFSAllegations_FK);
		}
	}
}