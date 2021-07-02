using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Threading;
using Infonet.Usps.Data.Models;

//KMS DO use AsNoTracking everywhere in here?
namespace Infonet.Usps.Data.Helpers {
	public class UspsHelper : IDisposable {
		#region constants
		public static readonly int IllinoisId = 14;
		#endregion

		#region static cache
		private static readonly States _OutOfCountry = new States { ID = 1936, StateAbbreviation = "Out of Country", StateName = "Out of Country" };

		private static readonly Counties _OutOfState = new Counties { ID = 1936, CountyName = "Out of Illinois" };

		private static readonly LazyHolder<IList<States>> _States = new LazyHolder<IList<States>>(() => {
			using (var db = new UspsContext())
				return LoadStates(db).AsReadOnly();
		});

		private static readonly LazyHolder<IList<States>> _StatesAndOutOfCountry = new LazyHolder<IList<States>>(() => {
			var results = new List<States>(_States.Value);
			results.Add(_OutOfCountry);
			return results.AsReadOnly();
		});

		private static readonly LazyHolder<States> _Illinois = new LazyHolder<States>(() => { return _States.Value.Single(s => s.ID == IllinoisId); });

		private static readonly LazyHolder<IList<Counties>> _IllinoisCounties = new LazyHolder<IList<Counties>>(() => {
			using (var db = new UspsContext())
				return LoadCountiesForState(db, IllinoisId).AsReadOnly();
		});

		private static readonly LazyHolder<IList<Counties>> _IllinoisCountiesAndOutOfIllinois = new LazyHolder<IList<Counties>>(() => {
			var results = new List<Counties>(_IllinoisCounties.Value);
			results.Add(_OutOfState);
			return results.AsReadOnly();
		});
		#endregion

		private UspsContext _db = null;

		#region constructing/disposing
		public UspsHelper() { }

		public UspsHelper(UspsContext context) {
			_db = context;
		}

		public void Dispose() {
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing)
				_db?.Dispose();
		}
		#endregion

		private UspsContext Db {
			get { return _db ?? (_db = new UspsContext()); }
		}

		public States OutOfCountry {
			get { return _OutOfCountry; }
		}

		public Counties OutOfState {
			get { return _OutOfState; }
		}

		public States Illinois {
			get { return _Illinois.Value; }
		}

		public IList<States> States {
			get { return _States.Value; }
		}

		public IList<States> StatesAndOutOfCountry {
			get { return _StatesAndOutOfCountry.Value; }
		}

		public IList<Counties> IllinoisCounties {
			get { return _IllinoisCounties.Value; }
		}

		public IList<Counties> IllinoisCountiesAndOutOfIllinois {
			get { return _IllinoisCountiesAndOutOfIllinois.Value; }
		}

		public States GetStateById(int stateId) {
			return Db.States.Single(s => s.ID == stateId);
		}

		public int GetStateIdByAbbreviation(string abbreviation) {
			return Db.States.Where(s => s.StateAbbreviation == abbreviation).Select(s => s.ID).Single();
		}

		public States GetStateByAbbreviation(string abbreviation) {
			return Db.States.Single(s => s.StateAbbreviation == abbreviation);
		}

		public IList<States> GetStatesByZip(string zip) {
			return Db.States.Where(s => s.ZipCodes.Any(z => z.Zipcode == zip)).OrderBy(s => s.StateAbbreviation).ToList();
		}

		public IList<Counties> GetCountiesByState(int stateId) {
			return stateId == Illinois.ID ? IllinoisCounties : LoadCountiesForState(Db, stateId);
		}

		public Counties GetCountyById(int CountyId) {
			return Db.Counties.FirstOrDefault(c => c.ID == CountyId); //KMS DO why not Single()?
		}

		public IList<Counties> GetCountiesByZip(string zip) {
			return Db.Counties.Where(s => s.ZipCodes.Any(z => z.Zipcode == zip)).ToList();
		}

		public IList<Cities> GetCitiesByZipCounty(string zip, int countyId) {
			return Db.Cities.Where(s => s.ZipCodes.Any(z => z.Zipcode == zip) && s.Counties.Any(c => c.ID == countyId)).ToList();
		}

		public IList<Cities> GetCitiesByZip(string zip) {
			return Db.Cities.Where(c => c.ZipCodes.Any(z => z.Zipcode == zip)).ToList();
		}

		public IList<Cities> GetCitiesByStateAndCounty(int? stateId, int? countyId) {
			string QUERY = @"SELECT DISTINCT dbo.Cities.ID, dbo.Cities.CityName 
                                FROM dbo.States 
                                JOIN dbo.StateXCounties 
                                ON dbo.StateXCounties.StateID = dbo.States.ID
                                JOIN dbo.Counties
                                ON dbo.Counties.ID = dbo.StateXCounties.CountyID
                                JOIN dbo.CountyXCities 
                                ON dbo.CountyXCities.CountyID = dbo.Counties.ID
                                JOIN dbo.Cities 
                                ON dbo.Cities.ID = dbo.CountyXCities.CityID
                                JOIN dbo.StateXZipcodes
                                ON dbo.StateXZipcodes.StateID = dbo.States.ID
                                JOIN dbo.CountyXZipcodes 
                                ON dbo.CountyXZipcodes.CountyID = dbo.Counties.ID
                                AND dbo.StateXZipcodes.Zipcode = dbo.CountyXZipcodes.Zipcode
                                JOIN dbo.CityXZipcodes
                                ON dbo.CityXZipcodes.CityID = dbo.Cities.ID
                                AND dbo.CityXZipcodes.Zipcode = dbo.CountyXZipcodes.Zipcode";

			return countyId.HasValue
				? Db.Database.SqlQuery<Cities>(QUERY + " WHERE dbo.States.ID = @p0 AND dbo.Counties.ID = @p1 ORDER BY dbo.Cities.CityName", stateId, countyId).ToList()
				: Db.Database.SqlQuery<Cities>(QUERY + " WHERE dbo.States.ID = @p0 ORDER BY dbo.Cities.CityName", stateId).ToList();
		}

		public IList<Townships> GetTownshipsByStateAndCounty(int? stateId, int? countyId) {
			string QUERY = @"SELECT DISTINCT dbo.Townships.ID, dbo.Townships.TownshipName
                                FROM dbo.States 
                                JOIN dbo.StateXCounties 
                                ON dbo.StateXCounties.StateID = dbo.States.ID
                                JOIN dbo.Counties
                                ON dbo.Counties.ID = dbo.StateXCounties.CountyID
                                JOIN dbo.CountyXTownships
                                ON dbo.CountyXTownships.CountyID = dbo.Counties.ID
                                JOIN dbo.Townships
                                ON dbo.Townships.ID = dbo.CountyXTownships.TownshipID
                                JOIN dbo.StateXZipcodes
                                ON dbo.StateXZipcodes.StateID = dbo.States.ID
                                JOIN dbo.CountyXZipcodes 
                                ON dbo.CountyXZipcodes.CountyID = dbo.Counties.ID
                                AND dbo.StateXZipcodes.Zipcode = dbo.CountyXZipcodes.Zipcode
                                JOIN dbo.TownshipXZipcodes 
                                ON dbo.TownshipXZipcodes.TownshipID = dbo.Townships.ID
                                AND dbo.TownshipXZipcodes.Zipcode = dbo.CountyXZipcodes.Zipcode";

			return countyId.HasValue
				? Db.Database.SqlQuery<Townships>(QUERY + " WHERE dbo.States.ID= @p0 AND dbo.Counties.ID = @p1", stateId, countyId).ToList()
				: Db.Database.SqlQuery<Townships>(QUERY + " WHERE dbo.States.ID= @p0", stateId).ToList();
		}

		public bool IsValidZip(string zip, int? countyId, int? stateId) {
			if (string.IsNullOrWhiteSpace(zip))
				return false;

			string value = zip.Trim();
			if (value.Length < 5)
				return false;

			countyId = countyId < 1936 ? countyId : 0;
			stateId = stateId < 1936 ? stateId : 0;
			// Remove suffix before validation, if present
			value = value.Substring(0, 5);
			var x = Db.ZipCodes.Where(z => z.Zipcode == value);
			if (countyId > 0)
				x = x.Where(z => z.Counties.Any(c => c.ID == countyId));
			if (stateId > 0)
				x = x.Where(z => z.States.Any(s => s.ID == stateId));
			return x.Any();
		}

		public IList<ListItem> SearchCityName(string input, int stateId, int countyId) {
			var cities = Db.Cities.Where(c => c.CityName.StartsWith(input));
			if (stateId > 0)
				cities = cities.Where(c => c.States.Any(s => s.ID == stateId));
			if (countyId > 0)
				cities = cities.Where(c => c.Counties.Any(co => co.ID == countyId));

			var results = new List<ListItem>();
			foreach (var each in cities.Take(5))
				results.Add(new ListItem(each.ID.ToString(), each.CityName));
			return results;
		}

		public IList<ListItem> SearchTownshipName(string input, int stateId, int countyId) {
			var townships = Db.Townships.Where(c => c.TownshipName.StartsWith(input) && c.States.Any(s => s.ID == stateId));
			if (countyId > 0)
				townships = townships.Where(c => c.Counties.Any(co => co.ID == countyId));

			var results = new List<ListItem>();
			foreach (var each in townships.Take(5))
				results.Add(new ListItem(each.ID.ToString(), each.TownshipName));
			return results;
		}

		public IList<ListItem> SearchZip(string input, int stateId, int countyId) {
			var zips = Db.ZipCodes.Where(c => c.Zipcode.StartsWith(input) && c.States.Any(s => s.ID == stateId));
			if (countyId > 0 && countyId < 1936)
				zips = zips.Where(c => c.Counties.Any(co => co.ID == countyId));

			var results = new List<ListItem>();
			foreach (ZipCodes zip in zips.Take(5))
				results.Add(new ListItem(zip.Zipcode, zip.Zipcode));
			return results;
		}

		//KMS DO convert this to wrap GetStatesByZip?
		//KMS DO then inline in USPSController?
		public IList<ListItem> ListStatesByZip(string zip) {
			var results = new List<ListItem>();
			foreach (var each in Db.States.Where(s => s.ZipCodes.Any(z => z.Zipcode == zip)).OrderBy(s => s.DisplayOrder))
				results.Add(new ListItem(each.ID.ToString(), each.StateAbbreviation));
			return results;
		}

		//KMS DO inline this in USPSController
		//KMS DO or just create new County object that doesn't have this problem
		//KMS DO still some wasted memory for Illinois counties
		public IEnumerable<ListItem> ListCountiesByState(int stateId) {
			return GetCountiesByState(stateId).Select(c => new ListItem(c.ID.ToString(), c.CountyName));
		}

		//KMS DO inline this too?
		public IList<ListItem> ListStatesCountiesCitiesByZip(string zip) {
			var results = new List<ListItem>();
			foreach (var eachState in Db.States.Where(s => s.ZipCodes.Any(z => z.Zipcode == zip)).OrderBy(s => s.DisplayOrder)) {
				var stateItem = new CompositeListItem(eachState.ID.ToString(), eachState.StateAbbreviation);
				results.Add(stateItem);
				foreach (var eachCounty in Db.Counties.Where(c => c.ZipCodes.Any(z => z.Zipcode == zip) && c.States.Any(s => s.ID == eachState.ID)).OrderBy(c => c.CountyName)) {
					var countyItem = new CompositeListItem(eachCounty.ID.ToString(), eachCounty.CountyName);
					stateItem.Children.Add(countyItem);
					foreach (var eachCity in Db.Cities.Where(city => city.ZipCodes.Any(z => z.Zipcode == zip) && city.States.Any(s => s.ID == eachState.ID) && city.Counties.Any(c => c.ID == eachCounty.ID)).OrderBy(city => city.CityName))
						countyItem.Children.Add(new ListItem(eachCity.ID.ToString(), eachCity.CityName));
				}
			}
			return results;
		}

		#region static utils
		public static void Reset() {
			_Illinois.Reset();
			_States.Reset();
			_StatesAndOutOfCountry.Reset();
			_IllinoisCounties.Reset();
		}

		private static List<States> LoadStates(UspsContext db) {
			return db.States.AsNoTracking().OrderBy(s => s.DisplayOrder).ToList();
		}

		private static List<Counties> LoadCountiesForState(UspsContext db, int stateId) {
			var results = db.Counties.AsNoTracking().Where(c => c.States.Any(s => s.ID == stateId)).OrderBy(c => c.CountyName).ToList();
			var unknown = results.SingleOrDefault(c => c.CountyName == "Unknown");
			if (unknown != null) {
				results.Remove(unknown);
				results.Add(unknown);
			}
			return results;
		}
		#endregion

		#region inner
		public class ListItem {
			public ListItem() { }

			public ListItem(string id, string name) {
				ID = id;
				Name = name;
			}

			public string ID { get; set; }

			public string Name { get; set; }
		}

		public class CompositeListItem : ListItem {
			public CompositeListItem() {
				Children = new List<ListItem>();
			}

			public CompositeListItem(string id, string name) : base(id, name) {
				Children = new List<ListItem>();
			}

			public IList<ListItem> Children { get; }
		}
		#endregion
	}
}