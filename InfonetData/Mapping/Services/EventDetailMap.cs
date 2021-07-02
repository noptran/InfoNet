using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Mapping.Services {
	public class EventDetailMap : EntityTypeConfiguration<EventDetail> {
		public EventDetailMap() {
			// Primary Key
			HasKey(t => t.ICS_ID);

			// Properties
			Property(t => t.EventName)
				.HasMaxLength(100);

			Property(t => t.Comment)
				.HasMaxLength(200);

			Property(t => t.Location)
				.HasMaxLength(100);

			// Table & Column Mappings
			ToTable("Tl_EventDetail");
			Property(t => t.ICS_ID).HasColumnName("ICS_ID");
			Property(t => t.CenterID).HasColumnName("CenterID");
			Property(t => t.ProgramID).HasColumnName("ProgramID");
			Property(t => t.EventName).HasColumnName("EventName");
			Property(t => t.EventHours).HasColumnName("EventHours");
			Property(t => t.NumPeopleReached).HasColumnName("NumPeopleReached");
			Property(t => t.EventDate).HasColumnName("EventDate");
			Property(t => t.Comment).HasColumnName("Comment");
			Property(t => t.Location).HasColumnName("Location");
			Property(t => t.FundDateID).HasColumnName("FundDateID");
			Property(t => t.CountyID).HasColumnName("CountyID");
			Property(t => t.StateID).HasColumnName("StateID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.Center)
				.WithMany(t => t.EventDetails)
				.HasForeignKey(d => d.CenterID);
			HasRequired(t => t.TLU_Codes_ProgramsAndServices)
				.WithMany(t => t.EventDetails)
				.HasForeignKey(d => d.ProgramID);
			// ADDED RELATIONSHIP
			HasOptional(t => t.FundingDate)
				.WithMany(t => t.EventsDetail)
				.HasForeignKey(d => d.FundDateID);
		}
	}
}