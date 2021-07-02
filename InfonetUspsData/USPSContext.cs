using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Infonet.Usps.Data.Helpers;
using Infonet.Usps.Data.Models;
using Serilog;

namespace Infonet.Usps.Data {
	public class UspsContext : DbContext {
		public UspsContext() : base("name=UspsContext") {
			Helper = new UspsHelper(this);
			Database.Log = LogAction;
		}

		public int? CommandTimeout {
			get { return ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;  }
			set { ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = value; }
		}

		public UspsHelper Helper { get; }

		public virtual DbSet<Abreviations> Abreviations { get; set; }
		public virtual DbSet<Cities> Cities { get; set; }
		public virtual DbSet<Counties> Counties { get; set; }
		public virtual DbSet<IllinoisGovernments> IllinoisGovernments { get; set; }
		public virtual DbSet<States> States { get; set; }
		public virtual DbSet<Townships> Townships { get; set; }
		public virtual DbSet<ZipcodePlus4> ZipcodePlus4 { get; set; }
		public virtual DbSet<ZipCodes> ZipCodes { get; set; }
		public virtual DbSet<CityTownships> CityTownships { get; set; }
		public virtual DbSet<CityZipcodes> CityZipcodes { get; set; }
		public virtual DbSet<CountyTownships> CountyTownships { get; set; }
		public virtual DbSet<CountyZipcodes> CountyZipcodes { get; set; }
		public virtual DbSet<StateCities> StateCities { get; set; }
		public virtual DbSet<StateCounties> StateCounties { get; set; }
		public virtual DbSet<StateCountyCityZipcode> StateCountyCityZipcode { get; set; }
		public virtual DbSet<StateCountyTownshipCityZipcode> StateCountyTownshipCityZipcode { get; set; }
		public virtual DbSet<StateTownships> StateTownships { get; set; }
		public virtual DbSet<StateZipcodes> StateZipcodes { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
			modelBuilder.Entity<Cities>()
				.HasMany(e => e.States)
				.WithMany(e => e.Cities)
				.Map(m => m.ToTable("StateXCities").MapLeftKey("CityID").MapRightKey("StateID"));

			modelBuilder.Entity<Cities>()
				.HasMany(e => e.Townships)
				.WithMany(e => e.Cities)
				.Map(m => m.ToTable("TownshipXCities").MapLeftKey("CityID").MapRightKey("TownshipID"));

			modelBuilder.Entity<Counties>()
				.HasMany(e => e.Cities)
				.WithMany(e => e.Counties)
				.Map(m => m.ToTable("CountyXCities").MapLeftKey("CountyID").MapRightKey("CityID"));

			modelBuilder.Entity<Counties>()
				.HasMany(e => e.Townships)
				.WithMany(e => e.Counties)
				.Map(m => m.ToTable("CountyXTownships").MapLeftKey("CountyID").MapRightKey("TownshipID"));

			modelBuilder.Entity<Counties>()
				.HasMany(e => e.States)
				.WithMany(e => e.Counties)
				.Map(m => m.ToTable("StateXCounties").MapLeftKey("CountyID").MapRightKey("StateID"));

			modelBuilder.Entity<States>()
				.Property(e => e.StateName)
				.IsUnicode(false);

			modelBuilder.Entity<States>()
				.Property(e => e.StateAbbreviation)
				.IsFixedLength();

			modelBuilder.Entity<States>()
				.HasMany(e => e.Townships)
				.WithMany(e => e.States)
				.Map(m => m.ToTable("StateXTownships").MapLeftKey("StateID").MapRightKey("TownshipID"));

			modelBuilder.Entity<States>()
				.HasMany(e => e.ZipCodes)
				.WithMany(e => e.States)
				.Map(m => m.ToTable("StateXZipcodes").MapLeftKey("StateID").MapRightKey("Zipcode"));

			modelBuilder.Entity<Townships>()
				.HasMany(e => e.ZipCodes)
				.WithMany(e => e.Townships)
				.Map(m => m.ToTable("TownshipXZipcodes").MapLeftKey("TownshipID").MapRightKey("Zipcode"));

			modelBuilder.Entity<ZipcodePlus4>()
				.Property(e => e.Zipcode)
				.IsFixedLength()
				.IsUnicode(false);

			modelBuilder.Entity<ZipcodePlus4>()
				.Property(e => e.Suffix)
				.IsFixedLength()
				.IsUnicode(false);

			modelBuilder.Entity<ZipCodes>()
				.Property(e => e.Zipcode)
				.IsFixedLength()
				.IsUnicode(false);

			modelBuilder.Entity<ZipCodes>()
				.HasMany(e => e.Cities)
				.WithMany(e => e.ZipCodes)
				.Map(m => m.ToTable("CityXZipcodes").MapLeftKey("Zipcode").MapRightKey("CityID"));

			modelBuilder.Entity<ZipCodes>()
				.HasMany(e => e.Counties)
				.WithMany(e => e.ZipCodes)
				.Map(m => m.ToTable("CountyXZipcodes").MapLeftKey("Zipcode").MapRightKey("CountyID"));

			modelBuilder.Entity<StateCities>()
				.Property(e => e.StateName)
				.IsUnicode(false);

			modelBuilder.Entity<StateCities>()
				.Property(e => e.StateAbbreviation)
				.IsFixedLength();

			modelBuilder.Entity<StateCounties>()
				.Property(e => e.StateName)
				.IsUnicode(false);

			modelBuilder.Entity<StateCounties>()
				.Property(e => e.StateAbbreviation)
				.IsFixedLength();

			modelBuilder.Entity<StateCountyCityZipcode>()
				.Property(e => e.StateAbbreviation)
				.IsFixedLength();

			modelBuilder.Entity<StateCountyCityZipcode>()
				.Property(e => e.Zipcode)
				.IsFixedLength()
				.IsUnicode(false);

			modelBuilder.Entity<StateCountyTownshipCityZipcode>()
				.Property(e => e.StateName)
				.IsUnicode(false);

			modelBuilder.Entity<StateTownships>()
				.Property(e => e.StateName)
				.IsUnicode(false);

			modelBuilder.Entity<StateTownships>()
				.Property(e => e.StateAbbreviation)
				.IsFixedLength();

			modelBuilder.Entity<StateZipcodes>()
				.Property(e => e.StateName)
				.IsUnicode(false);

			modelBuilder.Entity<StateZipcodes>()
				.Property(e => e.StateAbbreviation)
				.IsFixedLength();
		}

		/*
		 * Previously, intent was to have standard logging for all (Enhanced)DbContexts,
		 * but EF appears to have a memory leak that hangs onto the Database.Log Action<string>
		 * indefinintely.  Therefore, using a closure for the Action<string> leaks anything
		 * referenced by that closure.  If such a closure references a DbContext to access its
		 * datasource name, then that DbContext is leaked too.
		 */
		private static void LogAction(string s) {
			if (!string.IsNullOrWhiteSpace(s))
				Log.Verbose("{DbContext:l}: {Message:l}", "UspsContext", s.Trim());
		}
	}
}