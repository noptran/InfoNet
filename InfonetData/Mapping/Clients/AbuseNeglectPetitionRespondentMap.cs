using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class AbuseNeglectPetitionRespondentMap : EntityTypeConfiguration<AbuseNeglectPetitionRespondent> {
		public AbuseNeglectPetitionRespondentMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			// Table & Column Mappings
			ToTable("TS_AbuseNeglectPetitionsRespondents");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.AbuseNeglectPetition_FK).HasColumnName("AbuseNeglectPetitions_FK");
			Property(t => t.RespondentId).HasColumnName("RespondentID");
			Property(t => t.RespondentType).HasColumnName("RespondentType");

			// Relationships
			HasRequired(t => t.Petition)
				.WithMany(t => t.Respondents)
				.HasForeignKey(d => d.AbuseNeglectPetition_FK);
		}
	}
}