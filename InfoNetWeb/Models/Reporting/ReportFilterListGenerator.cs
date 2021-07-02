using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Infonet.Core.Collections;
using Infonet.Data;
using Infonet.Data.Looking;
using Infonet.Data.Models._TLU;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ViewModels;
using Infonet.Usps.Data;
using Infonet.Usps.Data.Helpers;
using Infonet.Web.Mvc;

namespace Infonet.Web.Models.Reporting {
	public class ReportFilterListGenerator {
		private readonly InfonetServerContext _db;
		private readonly UspsContext _usps;
		private readonly HttpSessionStateBase _session;

		public ReportFilterListGenerator(HttpSessionStateBase session, InfonetServerContext db, UspsContext usps) {
			_db = db;
			_usps = usps;
			_session = session;
		}

		public List<FilterSelection> GetLookupValues(Lookup lookup) {
			return lookup[_session.Center().Provider].Select(l => new FilterSelection { CodeId = l.CodeId.ToString(), Description = l.Description }).ToList();
		}

		#region Non-Lookups

		public List<FilterSelection> GetAgencies(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0)
				return null;

			var query = _db.T_Agency.Where(a => centerIds.Contains(a.CenterID ?? -999) || a.CenterID == 0);

			return query.Select(s => new FilterSelection {
				CodeId = s.AgencyID.ToString(),
				Description = s.AgencyName
			}).Distinct().OrderBy(d => d.Description).ToList();
		}

		public List<FilterSelection> GetCityOrTowns(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0)
				return null;

			int parentCenterId = _session.Center().Top.Id;
			if (!endDate.HasValue)
				endDate = DateTime.Today;

			return _db.Ts_TwnTshipCounty
				.Where(t => t.Client.CenterId == parentCenterId && t.MoveDate <= endDate.Value)
				.Select(s => new FilterSelection {
					CodeId = s.CityOrTown,
					Description = s.CityOrTown
				}).Distinct().OrderBy(o => o.Description).ToList();
		}

		public List<FilterSelection> GetCounties(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0)
				return null;
			int centerId = centerIds[0];
			int parentCenterId = _session.Center().Top.Id;
			List<FilterSelection> result;
			int? illinoisId = UspsHelper.IllinoisId;

			if (centerId > 1) {
				var query = _db.Ts_TwnTshipCounty
						.Where(t => t.Client.CenterId == parentCenterId && t.CountyID.HasValue);
				// PRC DO filter by date? What date to use?
				//if (endDate.HasValue) {
				//	query = query.Where(t => t.MoveDate <= endDate.Value);
				//}
				var countyIds = query.Select(s => new DualIdItem { IdOne = s.CountyID, IdTwo = s.StateID }).Distinct().ToList();

				var illinoisCountyIds = countyIds.Where(c => c.IdTwo == illinoisId).Select(c => c.IdOne).ToList();
				// Add Illinois Counties
				result = _usps.Counties
					.Where(c => illinoisCountyIds.Contains(c.ID))
					.Select(c => new FilterSelection {
						CodeId = c.ID.ToString(),
						Description = c.CountyName
					}).OrderBy(c => c.Description).ToList();

				var outOfStateCounties = new List<FilterSelection>();
				// Add Other Counties
				foreach (DualIdItem county in countyIds.Where(c1 => c1.IdTwo != illinoisId).OrderBy(c1 => c1.IdTwo)) {
					outOfStateCounties.AddRange(_usps.Counties
					.Where(c => county.IdOne == c.ID)
					.Select(c => new FilterSelection {
						CenterID = county.IdTwo ?? 0,
						CodeId = c.ID.ToString(),
						Description = "" + c.CountyName + (county.IdTwo.HasValue ? " (" + c.States.Where(s => s.ID == county.IdTwo).Select(s => s.StateAbbreviation).FirstOrDefault() + ")" : "N/A")
					}));
				}
				result.AddRange(outOfStateCounties.OrderBy(c => c.CenterID).ThenBy(c => c.Description));
			} else {
				result = _usps.Counties
				.Where(w => w.States.Select(s => s.ID).Contains(14))
				.Select(s => new FilterSelection {
					CodeId = s.ID.ToString(),
					Description = s.CountyName
				}).Distinct().OrderBy(o => o.Description).ToList();
			}
			return result;
		}

		public List<FilterSelection> GetEventLocations(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0)
				return null;

			var query = _db.Tl_EventDetail.Where(pd => centerIds.Contains(pd.CenterID));

			return query.Select(s => new FilterSelection {
				CodeId = s.Location,
				Description = s.Location
			}).Distinct().OrderBy(d => d.Description).ToList();
		}

		//KMS DO ignores DisplayOrder
		//KMS DO should group by Center
		public List<FilterSelection> GetFundingSources(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0) {
				return null;
			}
			return _db.TLU_Codes_FundingSource
				.Where(f => centerIds.Contains(f.CenterID.Value) || f.CenterID == 0)
				.Select(s => new FilterSelection {
					CodeId = (s.CodeID ?? 0).ToString(),
					Description = s.Description,
					CenterID = s.CenterID ?? -99
				})
				.OrderBy(d => d.Description).ToList();
		}

		//KMS DO ignores DisplayOrder
		//KMS DO should group by Center
		public List<FilterSelection> GetOtherStaffActivities(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0) {
				return null;
			}
			return _db.TLU_Codes_OtherStaffActivity
				.Where(f => centerIds.Contains(f.CenterID.Value) || f.CenterID == 0)
				.Select(s => new FilterSelection {
					CodeId = (s.CodeID ?? 0).ToString(),
					Description = s.Description,
					CenterID = s.CenterID ?? -99
				})
				.OrderBy(d => d.Description).ToList();
		}

		//KMS DO eliminate
		[Obsolete]
		public List<FilterSelection> GetPublications() {
			return _db.Database.SqlQuery<TLU_Codes_ProgramsAndServices>(@"
				SELECT * FROM TLU_Codes_ProgramsAndServices ps
				JOIN LOOKUPLIST_ItemAssignment ia
				ON ia.CodeID = ps.CodeID
				AND ia.ProviderID = @p0
				JOIN LOOKUPLIST_Tables t
				On t.TableID = ia.TableID
				AND t.TableName = 'TLU_Codes_ProgramsAndServices'
				WHERE ps.IsPublication = 1
			", _session.Center().ProviderId).Select(ps => new FilterSelection {
				CodeId = ps.CodeID.ToString(),
				Description = ps.Description
			}).Distinct().OrderBy(d => d.Description).ToList();
		}

		public List<FilterSelection> GetServices(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0)
				return null;

			if (!startDate.HasValue) {
				startDate = DateTime.Parse("01/01/1900");
			}
			if (!endDate.HasValue) {
				endDate = DateTime.Today;
			}

			var query = _db.Tl_ServiceDetailOfClient.Where(sd => centerIds.Contains(sd.LocationID ?? -999));
			query = query.Where(s => s.ServiceDate.HasValue && s.ServiceDate.Value >= startDate.Value && s.ServiceDate <= endDate.Value || s.ShelterBegDate.HasValue && s.ShelterBegDate.Value <= endDate.Value && (!s.ShelterEndDate.HasValue || s.ShelterEndDate >= startDate.Value));
			return query.Select(s => new FilterSelection {
				CodeId = s.ServiceID.ToString(),
				Description = s.TLU_Codes_ProgramsAndServices.Description
			}).Distinct().OrderBy(d => d.Description).ToList();
		}
        public List<FilterSelection> GetPrograms(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
            //if (centerIds == null || centerIds.Length <= 0)
            //    return null;

            //if (!startDate.HasValue)
            //    startDate = DateTime.Parse("01/01/1900");

            //if (!endDate.HasValue) 
            //    endDate = DateTime.Today;

            //var query = _db.Tl_ProgramDetail.Where(pd => centerIds.Contains(pd.CenterID));
            //query = query.Where(p => p.PDate.HasValue && p.PDate.Value >= startDate.Value && p.PDate <= endDate.Value);
            //return query.Select(p => new FilterSelection {
            //    CodeId = p.ProgramID.ToString(),
            //    Description = p.TLU_Codes_ProgramsAndServices.Description
            //}).Distinct().OrderBy(d => d.Description).ToList();

            return _db.Database.SqlQuery<TLU_Codes_ProgramsAndServices>(@"
				SELECT * FROM TLU_Codes_ProgramsAndServices ps
				JOIN LOOKUPLIST_ItemAssignment ia
				ON ia.CodeID = ps.CodeID
				AND ia.ProviderID = @p0
				JOIN LOOKUPLIST_Tables t
				On t.TableID = ia.TableID
				AND t.TableName = 'TLU_Codes_ProgramsAndServices'
				WHERE ps.IsCommInst = 1 OR IsGroupService = 1
			", _session.Center().ProviderId).Select(ps => new FilterSelection
            {
                CodeId = ps.CodeID.ToString(),
                Description = ps.Description
            }).Distinct().OrderBy(d => d.Description).ToList();
        }

        public List<FilterSelection> GetServiceLocations(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0)
				return null;

			var query = _db.Tl_ProgramDetail.Where(pd => centerIds.Contains(pd.CenterID));

			return query.Select(s => new FilterSelection {
				CodeId = s.Location,
				Description = s.Location
			}).Distinct().OrderBy(d => d.Description).ToList();
		}

		public List<FilterSelection> GetShelterType() {
			var clientTypes = new List<FilterSelection>();
			foreach (ShelterServiceEnum item in Enum.GetValues(typeof(ShelterServiceEnum))) {
				clientTypes.Add(new FilterSelection { CodeId = item.ToInt32().ToString(), Description = item.GetDisplayName() });
			}
			var sortOrder = new List<int> { -1, 66, 65, 118 };

			return clientTypes.OrderBy(c => sortOrder.IndexOf(Convert.ToInt32(c.CodeId))).ToList();
		}

		public List<FilterSelection> GetStaffNames(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0)
				return null;
			if (!startDate.HasValue)
				startDate = DateTime.Parse("01/01/1970");
			if (!endDate.HasValue)
				endDate = DateTime.Today;
			var staffList = _db.Helpers.Center.GetStaffForCentersAndDateRangeWithCenterName(startDate, endDate, centerIds)
				.OrderBy(s => s.EmployeeName)
				.Select(s => new FilterSelection {
					Description = s.EmployeeName,
					CodeId = s.SVID.ToString(),
					CenterID = s.CenterID
				}).ToList();
			int[] relatedCenterIds = _session.Center().AllRelated.Select(c => c.Id).ToArray();
			foreach (var staff in staffList)
				staff.CenterID = Array.IndexOf(relatedCenterIds, staff.CenterID);
			return staffList;
		}

		public List<FilterSelection> GetStates(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0)
				return null;
			int centerId = centerIds[0];
			int parentCenterId = _session.Center().Top.Id;
			var result = new List<FilterSelection>();

			if (!endDate.HasValue)
				endDate = DateTime.Today;

			if (centerId > 1) {
				var ids = _db.Ts_TwnTshipCounty
					.Where(t => t.Client.CenterId == parentCenterId && t.MoveDate <= endDate)
					.Select(t => t.StateID).Distinct().ToList();
				result = _usps.States
					.Where(s => ids.Contains(s.ID))
					.Select(s => new FilterSelection { CodeId = s.ID.ToString(), Description = s.StateAbbreviation })
					.OrderBy(s => s.Description)
					.ToList();
			} else if (centerId < 1 && centerIds.Length == 1) {
				result = _usps.States
					.Select(s => new FilterSelection { CodeId = s.ID.ToString(), Description = s.StateAbbreviation })
					.OrderBy(s => s.Description)
					.ToList();
			}
			return result;
		}

		public List<FilterSelection> GetTownships(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length <= 0)
				return null;

			int parentCenterId = _session.Center().Top.Id;
			if (!endDate.HasValue)
				endDate = DateTime.Today;

			return _db.Ts_TwnTshipCounty
				.Where(t => t.Client.CenterId == parentCenterId && t.MoveDate <= endDate.Value)
				.Select(s => new FilterSelection {
					CodeId = s.Township,
					Description = s.Township
				}).Distinct().OrderBy(o => o.Description).ToList();
		}

		public List<FilterSelection> GetZipcodes(int[] centerIds, DateTime? startDate = null, DateTime? endDate = null) {
			if (centerIds == null || centerIds.Length < 0)
				return null;

			int centerId = centerIds[0];
			int parentCenterId = _session.Center().Top.Id;
			if (!endDate.HasValue)
				endDate = DateTime.Today;

			var query = _db.Ts_TwnTshipCounty.Where(t => t.MoveDate <= endDate);
			if (centerId > 0)
                query = query.Where(t => t.Client.CenterId == parentCenterId && t.Zipcode != null);
            return query.Select(t => new FilterSelection {
				CodeId = t.Zipcode,
				Description = t.Zipcode
			}).Distinct().OrderBy(t => t.Description).ToList();
		}
		#endregion

		public static IEnumerable<SelectListItem> GetPdfSizeSelectList() {
			return PdfSizeEnum.BySize.Select(s => new SelectListItem {
				Text = s.GetDisplayName(),
				Value = s.ToInt32().ToString(),
				Selected = s == PdfSize.Letter
			});
		}

		public static IEnumerable<SelectListItem> GetPdfOrientationSelectList() {
			return Enums.GetValues<PdfOrientation>().Select(o => new SelectListItem {
				Text = o.GetDisplayName(),
				Value = o.ToInt32().ToString(),
				Selected = o == PdfOrientation.Portrait
			});
		}
	}

	public class DualIdItem {
		public int? IdOne { get; set; }
		public int? IdTwo { get; set; }
	}

}