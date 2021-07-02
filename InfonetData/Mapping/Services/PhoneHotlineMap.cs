using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Services {
	public class PhoneHotlineMap : EntityTypeConfiguration<PhoneHotline> {
		public PhoneHotlineMap() {
			// Primary Key
			HasKey(t => t.PH_ID);

			// Properties
			Property(t => t.Staff_Volunteer)
				.HasMaxLength(50);

			Property(t => t.Town)
				.HasMaxLength(50);

			Property(t => t.Township)
				.HasMaxLength(50);

			Property(t => t.ZipCode)
				.HasMaxLength(10);

			// Table & Column Mappings
			ToTable("T_PhoneHotline");
			Property(t => t.PH_ID).HasColumnName("PH_ID");
			Property(t => t.SVID).HasColumnName("SVID");
			Property(t => t.CenterID).HasColumnName("CenterID");
			Property(t => t.Date).HasColumnName("Date");
			Property(t => t.Staff_Volunteer).HasColumnName("Staff_Volunteer");
			Property(t => t.CallTypeID).HasColumnName("CallTypeID");
			Property(t => t.NumberOfContacts).HasColumnName("NumberOfContacts");
			Property(t => t.FundDateID).HasColumnName("FundDateID");
			Property(t => t.Town).HasColumnName("Town");
			Property(t => t.Township).HasColumnName("Township");
			Property(t => t.ZipCode).HasColumnName("ZipCode");
			Property(t => t.CountyID).HasColumnName("CountyID");
			Property(t => t.TimeOfDay).HasColumnName("TimeOfDay");
			Property(t => t.TotalTime).HasColumnName("TotalTime");
			Property(t => t.ReferralFromID).HasColumnName("ReferralFromID");
			Property(t => t.ReferralToID).HasColumnName("ReferralToID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.Age).HasColumnName("Age");
			Property(t => t.SexID).HasColumnName("SexID");
			Property(t => t.RaceID).HasColumnName("RaceID");
			Property(t => t.ClientTypeID).HasColumnName("ClientTypeID");

			// Relationships
			HasRequired(t => t.Center)
				.WithMany(t => t.PhoneHotlines)
				.HasForeignKey(d => d.CenterID);
			HasOptional(t => t.StaffVolunteer)
				.WithMany(t => t.PhoneHotlines)
				.HasForeignKey(d => d.SVID);
			HasOptional(t => t.FundingDate) 
				.WithMany(t => t.PhoneHotlines)
				.HasForeignKey(d => d.FundDateID);
		}
	}
}