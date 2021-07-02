using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;

namespace Infonet.Data.Helpers {
	public class FundingForStaffHelper {
		private readonly InfonetServerContext _db;

		internal FundingForStaffHelper(InfonetServerContext context) {
			_db = context;
		}

		public IEnumerable<FFSAssignedServices> GetAssignedServices(int? fundingDateId, int? staffId, DateTime? fundingDate) {
			if (fundingDate == null || fundingDate < SqlDateTime.MinValue.Value)
				return new List<FFSAssignedServices>();

			return _db.Database.SqlQuery<FFSAssignedServices>(@"SELECT A.*, TLUC_PAS.Code as Type, (TLUC_PAS.Description + ' (' + TLUC_PAS.Code + ')') as [Name] FROM (
				SELECT TL_FSPOS.ServiceProgramID, SUM(TL_FSPOS.PercentFund) as PercentFunded
				FROM TLU_Codes_FundingSource AS TLUC_FS
					INNER JOIN Tl_FundServiceProgramOfStaffs  AS TL_FSPOS
					ON TLUC_FS.CodeID = TL_FSPOS.FundingSourceID 
					WHERE(TL_FSPOS.FundDateID = {0}) 
					AND(TL_FSPOS.SVID = {1}) group by ServiceProgramID) AS A  
						INNER JOIN TLU_Codes_ProgramsAndServices AS TLUC_PAS on ServiceProgramID = TLUC_PAS.CodeID
						WHERE TLUC_PAS.Description != 'Unassigned'  
						ORDER BY Type DESC, Description, [Name] ", fundingDateId, staffId, fundingDate).ToList();
		}

		public IEnumerable<FFSStaff> GetAssignedStaff(int? fundingDateId) {
			if (fundingDateId == null)
				return new List<FFSStaff>();

			return _db.Database.SqlQuery<FFSStaff>(@"SELECT DISTINCT 
					Tl_FundServiceProgramOfStaffs.SVID, 
					T_StaffVolunteer.Type, 
					(T_StaffVolunteer.LastName + ', ' + T_StaffVolunteer.FirstName + ' (' + isnull(T_StaffVolunteer.Type, 'S') + ')') AS Name, 
					T_StaffVolunteer.TerminationDate  
				FROM Tl_FundServiceProgramOfStaffs INNER JOIN T_StaffVolunteer ON Tl_FundServiceProgramOfStaffs.SVID = T_StaffVolunteer.SVID  
					INNER JOIN T_FundingDates ON Tl_FundServiceProgramOfStaffs.FundDateID = T_FundingDates.FundDateID  
						AND T_StaffVolunteer.CenterID = T_FundingDates.CenterID  
					WHERE(Tl_FundServiceProgramOfStaffs.FundDateID = {0})  
						order by T_StaffVolunteer.Type,[Name]", fundingDateId).ToList();
		}

		public IEnumerable<FFSAvailableAndAssignedServices> GetAvailableAndAssignedServices(int? fundingDateId, int? staffId, int? providerId, DateTime? fundingDate) {
			if (fundingDate == null || fundingDate < SqlDateTime.MinValue.Value)
				return new List<FFSAvailableAndAssignedServices>();

			return _db.Database.SqlQuery<FFSAvailableAndAssignedServices>(@"SELECT A.ServiceProgramID, ('* ' + TLUC_PAS.Description + ' (' + TLUC_PAS.Code + ')') as [Name] , TLUC_PAS.Code, 0 as DisplayOrder, A.PercentFunded  FROM (
				SELECT TL_FSPOS.ServiceProgramID, SUM(TL_FSPOS.PercentFund) as PercentFunded
				FROM TLU_Codes_FundingSource AS TLUC_FS
					INNER JOIN Tl_FundServiceProgramOfStaffs  AS TL_FSPOS
					ON TLUC_FS.CodeID = TL_FSPOS.FundingSourceID 
					WHERE(TL_FSPOS.FundDateID = {0} ) 
					AND(TL_FSPOS.SVID = {1}) group by ServiceProgramID) AS A  
						INNER JOIN TLU_Codes_ProgramsAndServices AS TLUC_PAS on ServiceProgramID = TLUC_PAS.CodeID 
						WHERE TLUC_PAS.Description != 'Unassigned' 
				UNION 
					Select TLU_Codes_ProgramsAndServices.CodeID as ServiceProgramID 
						, TLU_Codes_ProgramsAndServices.Description + ' (' + isnull(TLU_Codes_ProgramsAndServices.Code, '') + ')' as [Name] 
						, Code, 1 as DisplayOrder, null AS PercentFunded 
					from TLU_Codes_ProgramsAndServices 
						inner join LookupList_ItemAssignment on 
							LookupList_ItemAssignment.codeID = TLU_Codes_ProgramsAndServices.CodeID 
							and LookupList_ItemAssignment.tableID = dbo.GetLookupListTableID('TLU_Codes_ProgramsAndServices') 
						where TLU_Codes_ProgramsAndServices.CodeID <> 0 
							and TLU_Codes_ProgramsAndServices.CodeID not in 
							( 
							SELECT distinct 
								Tl_FundServiceProgramOfStaffs.ServiceProgramID 
							FROM Tl_FundServiceProgramOfStaffs INNER JOIN 
							TLU_Codes_ProgramsAndServices ON 
							Tl_FundServiceProgramOfStaffs.ServiceProgramID = TLU_Codes_ProgramsAndServices.CodeID 
							inner join LookupList_ItemAssignment on 
							LookupList_ItemAssignment.codeID = TLU_Codes_ProgramsAndServices.CodeID 
							and LookupList_ItemAssignment.tableID = dbo.GetLookupListTableID('TLU_Codes_ProgramsAndServices') 
						WHERE(Tl_FundServiceProgramOfStaffs.FundDateID = {0}) AND 
							(Tl_FundServiceProgramOfStaffs.SVID = {1}) AND 
							(LookupList_ItemAssignment.ProviderID = {2}) 
							) 
							and(LookupList_ItemAssignment.ProviderID = {2}) 
							and TLU_Codes_ProgramsAndServices.codeid not in (65,66) 
							order by DisplayOrder, Code desc,[Name]", fundingDateId, staffId, providerId, fundingDate);
		}

		public IEnumerable<FFSStaff> GetAvailableAndAssignedStaff(int? fundingDateId, int? centerId, DateTime? fundingDate) {
			if (fundingDate == null || fundingDate < SqlDateTime.MinValue.Value)
				return new List<FFSStaff>();

			return _db.Database.SqlQuery<FFSStaff>(@"SELECT DISTINCT 
					t1.SVID AS SVID, 
					t1.Type AS Type, 
					t1.LastName + ', ' + t1.FirstName + ' (' + t1.Type + ')' AS Name, 
					1 as DisplayOrder 
				FROM T_StaffVolunteer t1 
					INNER JOIN T_FundingDates t2 ON t1.CenterID = t2.CenterID 
					WHERE t1.SVID NOT IN 
						(SELECT distinct T_StaffVolunteer.SVID 
						FROM T_StaffVolunteer 
						INNER JOIN T_FundingDates 
						ON T_StaffVolunteer.CenterID = T_FundingDates.CenterID 
			 			INNER JOIN Tl_FundServiceProgramOfStaffs ON 
							T_StaffVolunteer.SVID = Tl_FundServiceProgramOfStaffs.SVID AND 
							T_FundingDates.FundDateID = Tl_FundServiceProgramOfStaffs.FundDateID 
						 WHERE (T_FundingDates.FundDateID = {0}) 
							) 
							AND (t1.StartDate IS NULL OR {2} >= t1.StartDate ) AND (t1.TerminationDate IS NULL OR {2} <= t1.TerminationDate) 
							AND(t1.CenterID = {1}) 
						UNION 
						SELECT DISTINCT 
					Tl_FundServiceProgramOfStaffs.SVID AS SVID, 
					T_StaffVolunteer.Type AS Type, 
					('*' + T_StaffVolunteer.LastName + ', ' + T_StaffVolunteer.FirstName + ' (' + isnull(T_StaffVolunteer.Type, 'S') + ')') AS Name, 
					0 as DisplayOrder 
				FROM Tl_FundServiceProgramOfStaffs INNER JOIN T_StaffVolunteer ON Tl_FundServiceProgramOfStaffs.SVID = T_StaffVolunteer.SVID 
					INNER JOIN T_FundingDates ON Tl_FundServiceProgramOfStaffs.FundDateID = T_FundingDates.FundDateID 
						AND T_StaffVolunteer.CenterID = T_FundingDates.CenterID 
					WHERE (Tl_FundServiceProgramOfStaffs.FundDateID = {0}) 
						AND (T_StaffVolunteer.StartDate IS NULL OR {2} >= T_StaffVolunteer.StartDate ) AND (T_StaffVolunteer.TerminationDate IS NULL OR {2} <= T_StaffVolunteer.TerminationDate) 
						order by [DisplayOrder],[Type],[Name]", fundingDateId, centerId, fundingDate);
		}

		public IEnumerable<FFSFundIssueDatesString> GetFundIssueDates(int centerId) {
			return _db.Database.SqlQuery<FFSFundIssueDatesString>("SELECT replace(str(datepart(m,fundingdate)) + '/' + str(datepart(d,fundingdate)) + '/' + str(datepart(yyyy,fundingdate)),' ','')  as FundDate, FundDateID FROM T_FundingDates WHERE T_FundingDates.CenterID = {0} ORDER BY T_FundingDates.FundingDate DESC", centerId).ToList();
		}

		public IEnumerable<FFSFundIssueDatesString> GetFundIssueDatesBySvIdCenter(int? svid, int centerId) {
			return _db.Database.SqlQuery<FFSFundIssueDatesString>(
				@"SELECT  FundDateID, replace(str(datepart(m,fundingdate)) + '/' + str(datepart(d,fundingdate)) + '/' + str(datepart(yyyy,fundingdate)),' ','')  as FundDate
					FROM T_FundingDates WHERE CenterID = {1} AND FundDateID
						IN(SELECT DISTINCT FundDateID from Tl_FundServiceProgramOfStaffs 
							WHERE SVID = {0}) ORDER BY T_FundingDates.FundingDate DESC", svid, centerId);
		}

		public IEnumerable<FFSPercentFunded> GetPercentFunded(int? fundDateId) {
			if (fundDateId == null)
				return new List<FFSPercentFunded>();

			return _db.Database.SqlQuery<FFSPercentFunded>(@"SELECT Distinct 
					Tl_FundServiceProgramOfStaffs.SVID 
					, SUM(Tl_FundServiceProgramOfStaffs.PercentFund) AS PercentFunded 
				FROM Tl_FundServiceProgramOfStaffs INNER JOIN 
					T_FundingDates ON Tl_FundServiceProgramOfStaffs.FundDateID = T_FundingDates.FundDateID 
					INNER JOIN 
					TLU_Codes_ProgramsAndServices ON 
						Tl_FundServiceProgramOfStaffs.ServiceProgramID = TLU_Codes_ProgramsAndServices.CodeID 
				WHERE T_FundingDates.FundDateID = {0} 
					AND Tl_FundServiceProgramOfStaffs.ServiceProgramID <> 0 
					GROUP BY Tl_FundServiceProgramOfStaffs.SVID 
						, Tl_FundServiceProgramOfStaffs.ServiceProgramID 
						, TLU_Codes_ProgramsAndServices.Description 
						, TLU_Codes_ProgramsAndServices.Code 
					order by SVID", fundDateId).ToList();
		}

		public IEnumerable<FFSStaffFundedSources> GetStaffFundedSources(int? fundingDateId, int? svid, DateTime? selectedFundingDate) {
			if (selectedFundingDate == null || selectedFundingDate < SqlDateTime.MinValue.Value)
				return new List<FFSStaffFundedSources>();

			return _db.Database.SqlQuery<FFSStaffFundedSources>(@"SELECT * FROM (
				SELECT TLU_Codes_FundingSource.CodeID, Tl_FundServiceProgramOfStaffs.ServiceProgramID, TLU_Codes_FundingSource.Description, Tl_FundServiceProgramOfStaffs.PercentFund, TLU_Codes_FundingSource.EndDate
				FROM TLU_Codes_FundingSource 
					INNER JOIN Tl_FundServiceProgramOfStaffs 
					ON TLU_Codes_FundingSource.CodeID = Tl_FundServiceProgramOfStaffs.FundingSourceID 
					WHERE(Tl_FundServiceProgramOfStaffs.FundDateID = {0}) 
					AND Tl_FundServiceProgramOfStaffs.Percentfund >= 0 
					AND(Tl_FundServiceProgramOfStaffs.SVID = {1})) AS A  						  
						order by ServiceProgramID, Description", fundingDateId, svid, selectedFundingDate).ToList();
		}

		public IEnumerable<FFSStaffFundingSources> GetStaffFundingSources(int? centerId, int? fundingDateId, int? staffId, int? serviceId, int? providerId, DateTime? fundingDate) {
			if (fundingDate == null || fundingDate < SqlDateTime.MinValue.Value)
				return new List<FFSStaffFundingSources>();

			return _db.Database.SqlQuery<FFSStaffFundingSources>(@"SELECT DISTINCT 
						TLU_Codes_FundingSource.CodeID, 
						TLU_Codes_FundingSource.Description AS Name, 
						TLU_Codes_FundingSource.EndDate as EndDate, 
					(SELECT COUNT(FundingSourceID) 
					FROM Tl_FundServiceProgramOfStaffs 
					WHERE FundingSourceID = TLU_Codes_FundingSource.CodeID 
						AND FundDateID = {1}) AS IsSelected 
						, Tl_FundServiceProgramOfStaffs.PercentFund 
					FROM TLU_Codes_FundingSource LEFT OUTER JOIN 
						Tl_FundServiceProgramOfStaffs ON 
						TLU_Codes_FundingSource.CodeID = Tl_FundServiceProgramOfStaffs.FundingSourceID 
						inner join LookupList_ItemAssignment on 
					LookupList_ItemAssignment.codeID = TLU_Codes_FundingSource.CodeID 
						and LookupList_ItemAssignment.tableID = dbo.GetLookupListTableID('TLU_Codes_FundingSource') 
					WHERE(LookupList_ItemAssignment.providerID = {4}) 
						AND (Tl_FundServiceProgramOfStaffs.ServiceProgramID = {3}) 
						AND (Tl_FundServiceProgramOfStaffs.SVID = {2}) 
						AND (TLU_Codes_FundingSource.CodeID > 0) 
						AND Tl_FundServiceProgramOfStaffs.fundDateID = {1} 
						AND (LookupList_ItemAssignment.IsActive = 1 AND ({5} BETWEEN ISNULL(TLU_Codes_FundingSource.BeginDate, {5}) AND ISNULL(TLU_Codes_FundingSource.EndDate, {5})))
					Union 
					SELECT DISTINCT 
						TLU_Codes_FundingSource.CodeID, 
						TLU_Codes_FundingSource.Description AS Name, 
						TLU_Codes_FundingSource.EndDate as EndDate, 
						- 1 AS IsSelected, 
						0 as PercentFund 
					FROM TLU_Codes_FundingSource LEFT OUTER JOIN 
						Tl_FundServiceProgramOfStaffs ON 
						TLU_Codes_FundingSource.CodeID = Tl_FundServiceProgramOfStaffs.FundingSourceID 
					inner join LookupList_ItemAssignment on 
						LookupList_ItemAssignment.codeID = TLU_Codes_FundingSource.CodeID 
						and LookupList_ItemAssignment.tableID = dbo.GetLookupListTableID('TLU_Codes_FundingSource') 
					WHERE(LookupList_ItemAssignment.providerID = {4}) 
						AND (TLU_Codes_FundingSource.CodeID > 0) 
						AND ((TLU_Codes_FundingSource.CenterID = {0} 
						OR TLU_Codes_FundingSource.CenterID IS NULL 
						OR TLU_Codes_FundingSource.CenterID = 0)) 
						AND TLU_Codes_FundingSource.CodeID > 0 
						AND (LookupList_ItemAssignment.IsActive = 1 AND ({5} BETWEEN ISNULL(TLU_Codes_FundingSource.BeginDate, {5}) AND ISNULL(TLU_Codes_FundingSource.EndDate, {5})))
						AND TLU_Codes_FundingSource.CodeID not in 
						(select FundingSourceID from Tl_FundServiceProgramOfStaffs 
						where(FundDateID = {1} and SVID = {2} and serviceprogramid = {3}) 
						) 
					Order by Name", centerId, fundingDateId, staffId, serviceId, providerId, fundingDate).ToList();
		}

		public IEnumerable<FFSReportFundingHistory> ReportCurrentStaffFunding(int centerId, int staffId, int? fundDateId) {
			if (fundDateId == null)
				return new List<FFSReportFundingHistory>();

			return _db.Database.SqlQuery<FFSReportFundingHistory>(@"SELECT T_FundingDates.FundingDate
				,T_FundingDates.FundDateID
				,T_StaffVolunteer.LastName
				,T_StaffVolunteer.FirstName
				,(select Description + ' (' + Code + ')' from TLU_Codes_ProgramsAndServices where codeid = Tl_FundServiceProgramOfStaffs.ServiceProgramID) AS [ProgramOrService]
				,TLU_Codes_FundingSource.Description AS FundingSource
				,sum(Tl_FundServiceProgramOfStaffs.PercentFund) as PercentFund
				FROM Tl_FundServiceProgramOfStaffs INNER JOIN T_StaffVolunteer ON 
					Tl_FundServiceProgramOfStaffs.SVID = T_StaffVolunteer.SVID 
					INNER JOIN TLU_Codes_FundingSource ON 
						Tl_FundServiceProgramOfStaffs.FundingSourceID = TLU_Codes_FundingSource.CodeID
						INNER JOIN T_FundingDates ON 
							Tl_FundServiceProgramOfStaffs.FundDateID = T_FundingDates.FundDateID
							WHERE  T_FundingDates.CenterID = {0} AND
							(Tl_FundServiceProgramOfStaffs.FundDateID IN ({2})) AND 
    						(Tl_FundServiceProgramOfStaffs.SVID = {1}) 
							--AND (Tl_FundServiceProgramOfStaffs.FundingSourceID > 0)
						group By T_FundingDates.FundingDate,T_FundingDates.FundDateID,T_StaffVolunteer.LastName,T_StaffVolunteer.FirstName,Tl_FundServiceProgramOfStaffs.ServiceProgramID,TLU_Codes_FundingSource.Description with rollup
						having sum(Tl_FundServiceProgramOfStaffs.PercentFund) <= 100
						order by FundingDate desc,LastName,FirstName,ProgramOrService,FundingSource", centerId, staffId, fundDateId).ToList();
		}

		public IEnumerable<FFSReportFundingHistory> ReportCurrentStaffFundingAll(int centerId, int? fundDateId) {
			if (fundDateId == null)
				return new List<FFSReportFundingHistory>();

			return _db.Database.SqlQuery<FFSReportFundingHistory>(@"SELECT T_FundingDates.FundingDate
				,T_FundingDates.FundDateID
				,T_StaffVolunteer.LastName
				,T_StaffVolunteer.FirstName
				,(select Description + ' (' + Code + ')' from TLU_Codes_ProgramsAndServices where codeid = Tl_FundServiceProgramOfStaffs.ServiceProgramID) AS [ProgramOrService]
				,TLU_Codes_FundingSource.Description AS FundingSource
				,sum(Tl_FundServiceProgramOfStaffs.PercentFund) as PercentFund
				,Tl_FundServiceProgramOfStaffs.SVID
				FROM Tl_FundServiceProgramOfStaffs INNER JOIN T_StaffVolunteer ON 
					Tl_FundServiceProgramOfStaffs.SVID = T_StaffVolunteer.SVID 
					INNER JOIN TLU_Codes_FundingSource ON 
						Tl_FundServiceProgramOfStaffs.FundingSourceID = TLU_Codes_FundingSource.CodeID
						INNER JOIN T_FundingDates ON 
							Tl_FundServiceProgramOfStaffs.FundDateID = T_FundingDates.FundDateID
							where T_FundingDates.CenterID = {0} 
							--and  Tl_FundServiceProgramOfStaffs.FundingSourceID > 0
							and T_FundingDates.FundDateID = {1}
						group By T_FundingDates.FundingDate,T_FundingDates.FundDateID,Tl_FundServiceProgramOfStaffs.SVID,T_StaffVolunteer.LastName,T_StaffVolunteer.FirstName,Tl_FundServiceProgramOfStaffs.ServiceProgramID,TLU_Codes_FundingSource.Description with rollup
						having sum(Tl_FundServiceProgramOfStaffs.PercentFund) <= 100
						order by FundingDate desc,FundDateID, LastName,FirstName,ProgramOrService,FundingSource", centerId, fundDateId).ToList();
		}

		public int? GetFundDateId(DateTime date, int centerId) {
			return _db.T_FundingDates.Where(fd => fd.Date <= date && fd.CenterID == centerId).OrderByDescending(fd => fd.Date).Select(fd => (int?)fd.FundDateID).FirstOrDefault();
		}
	}

	public class FFSFundIssueDatesString {
		public string FundDate { get; set; }
		public int? FundDateID { get; set; }
	}

	public class FFSStaff {
		public int? SVID { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
		public DateTime? TerminationDate { get; set; }
	}

	public class FFSAssignedServices : FFSServiceProgramIDName {
		public string Type { get; set; }
		public int? PercentFunded { get; set; }
		public bool IsDeleted { get; set; }
	}

	public class FFSAvailableAndAssignedServices : FFSAvailableServices {
		public int? PercentFunded { get; set; }
	}

	public class FFSAvailableServices : FFSServiceProgramIDName {
		public string Code { get; set; }
	}

	public class FFSPercentFunded {
		public int? SVID { get; set; }
		public int? PercentFunded { get; set; }
	}

	public class FFSStaffFundedSources {
		public int? CodeID { get; set; }
		public int? ServiceProgramID { get; set; }
		public string Description { get; set; }
		public short? PercentFund { get; set; }
	}

	public class FFSCodeIDName {
		public int? CodeID { get; set; }
		public string Name { get; set; }
	}

	public class FFSStaffFundingSources : FFSCodeIDName {
		public DateTime? EndDate { get; set; }

		[Display(Name = "Percent")]
		[Range(0, 100)]
		public int? PercentFund { get; set; }
	}

	public class FFSServiceProgramIDName {
		public int? ServiceProgramID { get; set; }
		public string Name { get; set; }
	}

	public class FFSReportFundingHistory {
		public DateTime? FundingDate { get; set; }
		public int? FundDateID { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string ProgramOrService { get; set; }
		public string FundingSource { get; set; }
		public int? PercentFund { get; set; }
		public int? SVID { get; set; }
	}
}