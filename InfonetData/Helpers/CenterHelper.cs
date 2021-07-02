using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using Infonet.Data.Looking;

namespace Infonet.Data.Helpers {
	public class CenterHelper {
		private readonly InfonetServerContext _db;

		internal CenterHelper(InfonetServerContext context) {
			_db = context;
		}

		//KMS DO move to UsersAdminController?
		public IEnumerable<CenterInfo> GetAllCenters() {
			return _db.T_Center.Select(c => new CenterInfo { CenterId = c.CenterID, CenterName = c.CenterName }).OrderBy(c => c.CenterName);
		}

		//KMS DO move to UsersAdminController?  usage in ReportContainer should probably be replaced
		public CenterInfo GetCenterById(int id) {
			return _db.T_Center.Where(c => c.CenterID == id).Select(c => new CenterInfo { CenterId = c.CenterID, CenterName = c.CenterName, Provider = (Provider)c.ProviderID }).Single();
		}

		public IList<Staff> GetAllStaffForCenter(int centerId) {
			return _db.Database.SqlQuery<Staff>("SELECT SVID As SVID , LastName + ', ' + FirstName as [EmployeeName], TerminationDate FROM T_StaffVolunteer WHERE Centerid = @p0 ORDER BY [EmployeeName]", centerId).ToList();
		}

		public IList<Staff> GetStaffForCenterAndDate(string serviceDate, int centerId) {
			return GetStaffForCenterAndDate(string.IsNullOrEmpty(serviceDate) ? (DateTime?)null : DateTime.Parse(serviceDate), centerId);
		}

		public IList<Staff> GetStaffForCenterAndDate(DateTime? serviceDate, int centerId) {
			return _db.Database.SqlQuery<Staff>("SELECT SVID As SVID , LastName + ', ' + FirstName as [EmployeeName] FROM T_StaffVolunteer WHERE Centerid = @p1 AND (StartDate IS NULL OR StartDate <= @p0) AND (TerminationDate IS NULL OR TerminationDate > @p0) ORDER BY [EmployeeName]", serviceDate, centerId).ToList();
		}

		public IList<Staff> GetPaidStaffForCenterAndDate(DateTime? serviceDate, int centerId) {
			return _db.Database.SqlQuery<Staff>("SELECT SVID As SVID , LastName + ', ' + FirstName as [EmployeeName] FROM T_StaffVolunteer WHERE Centerid = @p1 AND (TypeID = 1 OR Type = 'S') AND (StartDate IS NULL OR StartDate <= @p0) AND (TerminationDate IS NULL OR TerminationDate > @p0) ORDER BY [EmployeeName]", serviceDate, centerId).ToList();
		}

		public IList<Staff> GetStaffForCentersAndDateRangeWithCenterName(DateTime? startDate, DateTime? endDate, int[] centerIds) {
			var start = startDate.HasValue && startDate.Value >= (DateTime)SqlDateTime.MinValue ? startDate.Value : (DateTime)SqlDateTime.MinValue;
			var end = endDate ?? DateTime.Today;
			return _db.Database.SqlQuery<Staff>("SELECT DISTINCT SVID As SVID , LastName + ', ' + FirstName + ' (' + c.CenterName + ')' as [EmployeeName], c.CenterID FROM T_StaffVolunteer s JOIN T_Center c ON c.CenterID = s.CenterId WHERE s.Centerid IN (" + string.Join(", ", centerIds) + ") AND((s.StartDate IS NULL OR s.StartDate <= @p1) AND (s.TerminationDate IS NULL OR (s.TerminationDate >= @p0))) ORDER BY[EmployeeName]", start, end).ToList();
		}

		public Staff GetStaffFromSvId(int svid) {
			return _db.Database.SqlQuery<Staff>("SELECT SVID As SVID , LastName + ', ' + FirstName as [EmployeeName] FROM T_StaffVolunteer WHERE SVID = @p0", svid).SingleOrDefault();
		}

		public IEnumerable<Staff> GetStaffForCenterAndDateRetainCurrentSvid(DateTime? serviceDate, int centerId, int? currentSvid) {
            List<Staff> staff = new List<Staff>();
            if (!serviceDate.HasValue) 
                serviceDate = DateTime.Now;
            staff = _db.Database.SqlQuery<Staff>("SELECT SVID As SVID , LastName + ', ' + FirstName as [EmployeeName] FROM T_StaffVolunteer WHERE Centerid = @p1 AND (StartDate IS NULL OR StartDate <= @p0) AND (TerminationDate IS NULL OR TerminationDate > @p0) ORDER BY [EmployeeName]", serviceDate, centerId).ToList();
            if (currentSvid != null && currentSvid > 0 && staff.FindIndex(f => f.SVID == currentSvid) == -1)
				staff.Insert(0, GetStaffFromSvId((int)currentSvid));
			return staff;
		}

        public IEnumerable<Staff> GetStaffForCentersAndDateRange(DateTime? startDate, DateTime? endDate, int centerId) {
            var start = startDate.HasValue && startDate.Value >= (DateTime)SqlDateTime.MinValue ? startDate.Value : (DateTime)SqlDateTime.MinValue;
            var end = endDate ?? DateTime.Today;
            return _db.Database.SqlQuery<Staff>("SELECT SVID As SVID , LastName + ', ' + FirstName as [EmployeeName] FROM T_StaffVolunteer WHERE Centerid = @p2 AND (StartDate IS NULL OR StartDate <= @p1) AND (TerminationDate IS NULL OR (TerminationDate >= @p0 AND TerminationDate <= @p1)) ORDER BY [EmployeeName]", start,end, centerId).ToList();
        }

        public IEnumerable<AgencyListItem> GetAgencyForCenterinCurrentAgencyId(int providerId, int centerId, int? currentAgencyId) {
            List<AgencyListItem> agency = new List<AgencyListItem>();

            agency = _db.Database.SqlQuery<AgencyListItem>("SELECT dbo.T_Agency.AgencyID, dbo.T_Agency.AgencyName FROM dbo.LOOKUPLIST_Tables INNER JOIN dbo.LOOKUPLIST_ItemAssignment ON dbo.LOOKUPLIST_Tables.TableID = dbo.LOOKUPLIST_ItemAssignment.TableID INNER JOIN dbo.T_Agency ON dbo.LOOKUPLIST_ItemAssignment.CodeID = dbo.T_Agency.AgencyID WHERE dbo.LOOKUPLIST_ItemAssignment.ProviderID = @p0 AND dbo.LOOKUPLIST_ItemAssignment.IsActive = 1 AND dbo.LOOKUPLIST_ItemAssignment.TableID = 48 AND(dbo.T_Agency.Centerid = 0 OR dbo.T_Agency.CenterID in(@p1)) ORDER BY dbo.LOOKUPLIST_ItemAssignment.DisplayOrder, dbo.T_Agency.AgencyName", providerId, centerId).ToList();
            if (currentAgencyId != null && currentAgencyId > 0 && agency.FindIndex(f => f.AgencyID == currentAgencyId) == -1)
                agency.Insert(0, GetAgencyFromId((int)currentAgencyId));
            return agency;
        }

        public AgencyListItem GetAgencyFromId(int AgencyId) {
            return _db.Database.SqlQuery<AgencyListItem>("SELECT dbo.T_Agency.AgencyID, dbo.T_Agency.AgencyName FROM dbo.T_Agency WHERE AgencyID = @p0", AgencyId).SingleOrDefault();
        }

    }
}