using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Infonet.Core;
using Infonet.Core.Data;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Importing {
	public sealed class ServicesImport : IDisposable {
		#region constants
		private const string LOG_DBL_HORIZONTAL_BAR = "=================================================================================================================================================";
		private const string LOG_SGL_HORIZONTAL_BAR = "-------------------------------------------------------------------------------------------------------------------------------------------------";
		private const string MSG_ERR_CANNOT_OVERWRITE_EXISTING_FILE = "File already exists: {0}.";
		private const string MSG_ERR_INVALID_IMPORT_TABLE_SCHEMA = "ERROR: Import table [{0}] schema does not match the expected format.";
		private const string MSG_ERR_NO_PRIMARY_KEYS_DEFINED_FOR_TABLE = "Dataset Table: {0} requires primary key definitions for update.";
		private const string MSG_ERR_NO_RECORDS_IN_DATASET_TABLE_TO_IMPORT = "Data Table: {0} does not contain any records to import.";
		private const string MSG_ERR_TABLE_NOT_LOADED_IN_DATASET = "Dataset Table: {0} does not exist.";
		private const string MSG_LOCATIONID_MUST_MATCH_CENTERID = "LocationID must be {0}.";
		private const string MSG_NONSHELTER_SERVICES_REQUIRE_NONZERO_RECEIVED_HOURS = "Non-shelter service requires non-zero value for received hours.";
		private const string MSG_NONSHELTER_SERVICES_REQUIRE_NULL_SHELTER_DATES = "Non-Shelter services must have Null Shelter begin and end dates.";
		private const string MSG_NONSHELTER_SERVICES_REQUIRE_SERVICEDATE = "Non-shelter service requires service date.";
        private const string MSG_NONSHELTER_SERVICES_REQUIRE_SVID = "Non-shelter service requires SVID.";
        private const string MSG_PROGRAM_DETAIL_REQUIRES_NUMOFSESSIONS = "Program Detail record requires a valid non-zero num of sessions.";
		private const string MSG_SERVICEDATE_MUST_BE_ON_OR_AFTER_FIRSTCONTACTDATE = "Service or shelter dates must be on or after first contact date: ";
        private const string MSG_SERVICEID_REQUIRED = "Service requires ServiceID.";
        private const string MSG_SHELTER_SERVICE_BEGINDATE_REQUIRED = "Shelter service requires shelter begin date.";
        private const string MSG_SHELTER_SERVICE_RECEIVEDHOURS_REQUIRES_ZERO = "Shelter service received hours requires 0.";
        private const string MSG_SHELTER_CONTAINS_NON_SHELTER_DATA = "Shelter service cannot contain service date or received hours.";
		private const string MSG_SHELTER_BEGIN_MUST_BE_ON_OR_BEFORE_SHELTER_END = "Shelter begin date must be less than or equal to shelter end date.";
		private const string MSG_PROGRAM_DETAIL_REQUIRES_PDATE = "Program Detail record requires a Pdate.";
		private const string TABLE_EXCEPTION_RECORDS = "ExceptionRecords";
		private const string TABLE_EXCEPTION_RECORDS_STAFF = "ExceptionRecordsStaff";
		private const string TABLE_EXCEPTION_RECORDS_PGMDETAIL = "ExceptionRecordsPgmDetail";
		private const string TABLE_CLIENTID_CASEID = "T_ClientCases";
		private const string TABLE_FUNDING_DATE_IDS = "T_FundingDates";
		private const string TABLE_PROGRAMS_AND_SERVICES = "TLU_Codes_ProgramsAndServices";
		private const string TABLE_PROGRAMS_AND_SERVICESGROUP = "TLU_Codes_ProgramsAndServicesGroup";
		private const string TABLE_SERVICEDETAIL_IMPORT = "T_ServiceDetailOfClient";
		private const string TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL = "T_ServiceDetailOfClientIndividual";
		private const string TABLE_STAFF_VOLUNTEER = "T_StaffVolunteer";
		private const string TABLE_TOWNTOWNSHIPCOUNTY = "Ts_TwnTshipCounty";
		private const string TABLE_PROGRAMDETAIL_IMPORT = "T_ProgramDetail";
		private const string TABLE_PROGRAMDETAIL_STAFFS_IMPORT = "T_ProgramDetail_Staffs";
		private const string TABLE_SQL_SERVICEDETAIL = "Tl_ServiceDetailOfClient";
		private const string TABLE_SQL_PROGRAMDETAIL = "Tl_ProgramDetail";
		private const string TABLE_SQL_PROGRAMDETAIL_STAFFS = "Ts_ProgramDetail_Staffs";
		#endregion constants

		#region static readonly
		private static readonly DataColumn[] _ImportTableDataColumns = {
			new DataColumn("ClientID", typeof(int)),
			new DataColumn("CaseID", typeof(int)),
			new DataColumn("ServiceID", typeof(int)),
			new DataColumn("SVID", typeof(int)),
			new DataColumn("ServiceDate", typeof(DateTime)),
			new DataColumn("LocationID", typeof(int)),
			new DataColumn("ReceivedHours", typeof(double)),
			new DataColumn("ShelterBegDate", typeof(DateTime)),
			new DataColumn("ShelterEndDate", typeof(DateTime)),
			new DataColumn("AgencyRecID", typeof(int)),
			new DataColumn("Agency_ICS_ID", typeof(int))
		};

		private static readonly DataColumn[] _ImportTableDataColumnsPgmDetail = {
			new DataColumn("Agency_ICS_ID", typeof(int)),
			new DataColumn("ProgramID", typeof(int)),
			new DataColumn("NumOfSessions", typeof(int)),
			new DataColumn("PDate", typeof(DateTime)),
			new DataColumn("ParticipantsNum", typeof(int)),
			new DataColumn("Hours", typeof(double))
		};

		private static readonly DataColumn[] _ImportTableDataColumnsPgmStaffs = {
			new DataColumn("Agency_ICS_ID", typeof(int)),
			new DataColumn("SVID", typeof(int)),
			new DataColumn("HoursOfConduct", typeof(double)),
			new DataColumn("HoursPrepare", typeof(double)),
			new DataColumn("HoursTravel", typeof(double))
		};
		#endregion static readonly

		#region fields
		private readonly int _centerId;
		private readonly int _providerId;
		private readonly string _mdbPath;
		private /*readonly*/ SqlCommandFactory _infonet;
		private /*readonly*/ DataSet _dataset;

		private readonly Queue _invalidImportRows = new Queue(); //KMS DO rename
		private /*lazy readonly*/ StreamWriter _log = null;

		private DateTime _started;
		private bool _importedServiceRecords = false;
		private bool _importedServiceRecordsGroupProgramDetail = false;
		private bool _importedServiceRecordsGroupProgramDetailStaff = false;
		private bool _importedServiceRecordsGroupProgramDetailClient = false;
		private int _importRecordCount = 0;
		private int _addRecordCount = 0;
		private int _updateRecordCount = 0;

		private readonly Dictionary<int, ProgramDetailSummary> _icsIdTable = new Dictionary<int, ProgramDetailSummary>();
		#endregion fields

		//KMS DO need copy of prod app.config for DataMigrationService...
		//<add key="ProcessSleepInterval" value="1" />
		//<add key="InfonetServerDatabase_ConnectionString" value="data source=localhost;initial catalog=InfonetDevelopment;user id=Infonet;password=Infonet;persist security info=True;workstation id=CRTLG01;packet size=4096;Connect Timeout=60" />
		//<add key="InfonetServerDatabase_CommandTimeout" value="60" />
		//<add key="JetOLEDBVersion" value="Microsoft.Jet.OleDb.4.0" />
		//<add key="ServiceDetailImport.FileSystemWatcher.Filter" value="ServiceDetail_*.mdb" />
		//<add key="ServiceDetailImport.FileSystemWatcher.DeleteMDBFileAfterImport" value="true" />
		//<add key="ValidateOnly" value="false" />
		//<add key="AssignTownTownShipCountyID" value="true"/>
		//<add key="AssignFundingDateID" value="true"/>
		//<add key="AllowServiceBeforeFirstContact" value="true"/>
		//<add key="AllowZeroServiceHours" value="false"/>
		//KMS DO any other than above?

		#region constructing/disposing
		public ServicesImport(int providerId, int centerId, string mdbPath, string infonetConnectionString, int sqlCommandTimeout) {
			if (string.IsNullOrEmpty(mdbPath) || !File.Exists(mdbPath))
				throw new ArgumentException($"{nameof(mdbPath)} is null, empty, or does not exist");

			_providerId = providerId;
			_centerId = centerId;
			_mdbPath = mdbPath;

			_infonet = new SqlCommandFactory(new SqlConnection(infonetConnectionString), sqlCommandTimeout);
			_infonet.Connection.Open();

			_dataset = new DataSet("Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + _mdbPath);
			if (sqlCommandTimeout >= 0)
				_dataset.SqlCommandTimeout = (short)sqlCommandTimeout; //KMS DO does anything?
		}

		private void Dispose(bool disposing) {
			if (disposing) {
				if (_dataset != null) {
					_dataset.Dispose();
					_dataset = null;
				}
				if (_infonet != null) {
					_infonet.Connection.Dispose();
					_infonet = null;
				}
				if (_log != null) {
					_log.Dispose();
					_log = null;
				}
			}
		}

		public void Dispose() {
			Dispose(true);
		}
		#endregion constructing/disposing

		#region logging
		/** Returns message parameter so it may be passed thru to exceptions and such. **/

		private string Log(string message, bool includeTimeStamp = true) {
			if (_log == null)
				_log = new StreamWriter(new FileStream(Path.ChangeExtension(_mdbPath, "log"), File.Exists(Path.ChangeExtension(_mdbPath, "log")) ? FileMode.Append : FileMode.CreateNew, FileAccess.Write, FileShare.Read), Encoding.UTF8);

			if (includeTimeStamp) {
				var now = DateTime.Now;
				_log.Write($"{now:M/d/yyyy} {now.ToLongTimeString()} | "); //KMS DO what is long time format string?
			}
			_log.WriteLine(message);
			_log.Flush();
			return message;
		}

		private void LogException(Exception ex, bool includeStackTrace = true, bool includeInnerException = true, bool isInner = false) {
			if (!isInner)
				WriteSingleBarToLog();
			Log(isInner ? "Inner Exception: " : "Exception: " + ex.GetType());
			Log("Message: " + ex.Message);
			Log("Source: " + ex.Source);
			if (includeStackTrace) {
				Log("Stack Trace: ");
				Log(ex.StackTrace);
			}
			if (includeInnerException && ex.InnerException != null)
				LogException(ex.InnerException, includeStackTrace, true, true);
			if (!isInner)
				WriteSingleBarToLog();
		}

		private static string PrintElapsedTime(DateTime startTime, DateTime? endTime = null) {
			if (endTime == null)
				endTime = DateTime.Now;

			var elapsed = (DateTime)endTime - startTime;
			var sb = new StringBuilder();
			if (elapsed.Days > 0)
				sb.Append(elapsed.Days).Append(elapsed.Days > 1 ? " days " : " day ");
			if (elapsed.Hours > 0)
				sb.Append(elapsed.Hours).Append(" hr ");
			if (elapsed.Minutes > 0)
				sb.Append(elapsed.Minutes).Append(" min ");
			return sb.AppendFormat("{0:0.###} sec", elapsed.TotalSeconds - (int)elapsed.TotalMinutes * 60).ToString();
		}

		private void WriteDoubleBarToLog() {
			Log(LOG_DBL_HORIZONTAL_BAR, false);
		}

		private void WriteSingleBarToLog() {
			Log(LOG_SGL_HORIZONTAL_BAR, false);
		}

		private string GetFileProcessingStatsMessage() {
			int validRecordCount = GetTableRowCount(TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL) +
									GetTableRowCount(TABLE_PROGRAMDETAIL_IMPORT) +
									GetTableRowCount(TABLE_PROGRAMDETAIL_STAFFS_IMPORT) +
									GetTableRowCount(TABLE_SERVICEDETAIL_IMPORT);
			int invalidRecordCount = GetTableRowCount(TABLE_EXCEPTION_RECORDS) +
									GetTableRowCount(TABLE_EXCEPTION_RECORDS_STAFF) +
									GetTableRowCount(TABLE_EXCEPTION_RECORDS_STAFF) +
									GetTableRowCount(TABLE_EXCEPTION_RECORDS_PGMDETAIL);

			var now = DateTime.Now;
			var message = new StringBuilder();
			message.AppendLine("Valid Record Count: " + validRecordCount);
			message.AppendLine("Invalid Record Count: " + invalidRecordCount);
			message.AppendLine("Records Added: " + _addRecordCount);
			message.AppendLine("Records Updated: " + _updateRecordCount);
			message.AppendLine("Total Records Imported: " + _importRecordCount);
			message.AppendLine($"Records per Second: {_importRecordCount / Math.Max((now - _started).TotalSeconds, double.Epsilon):0.###}");
			message.Append($"Total Elapsed Time: {PrintElapsedTime(_started, now)}");
			return message.ToString();
		}

		private int GetTableRowCount(string tableName) {
			return !_dataset.Tables.Contains(tableName) ? 0 : _dataset.Tables[tableName].Rows.Count;
		}

		public void WriteStatisticsToLog() {
			WriteDoubleBarToLog();
			Log("Calculating Import Statistics: " + Path.GetFileName(_mdbPath));
			WriteSingleBarToLog();

			RemoveInvalidRows(); //KMS DO does this need to be done?  if so, here?
			RemoveInvalidRows(); //KMS DO any reason to keep calling this twice?
			RemoveInvalidRowsStaff();
			RemoveInvalidRowsProgramDetail();

			Log(GetFileProcessingStatsMessage(), false);
			WriteDoubleBarToLog();
		}
		#endregion logging

		#region loading and validating
		public void LoadAndValidate() {
			string mdbName = Path.GetFileName(_mdbPath);
			WriteDoubleBarToLog();
			Log(string.Format("{0}: {1}", "Loading and Validating", mdbName));
			WriteSingleBarToLog();

			try {
				_started = DateTime.Now;
				ImportServiceRecords();
				WriteSingleBarToLog();
				ImportServiceRecordsGroupProgramDetail();
				WriteSingleBarToLog();
				ImportServiceRecordsGroupProgramDetailStaff();
				WriteSingleBarToLog();
				ImportServiceRecordsGroupProgramDetailClient();
			} catch (Exception e) {
				LogException(e);
				Log($"Services Import failed: unrecognized error occurred while {"Loading and Validating".ToLower()} {mdbName}");
				throw;
			}
		}

		private void ImportServiceRecords() {
			_importedServiceRecords = false;
			var stepStart = DateTime.Now;
			if (_dataset.Tables.Contains(TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL))
				_dataset.Tables.Remove(TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL);
			if (LoadServiceDetailOfClientImportTable()) {
				Log($"Table loaded: {TABLE_SERVICEDETAIL_IMPORT} ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				LoadCenterProgramsAndServices();
				Log($"Center programs and services loaded. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				LoadCenterProgramsAndServicesGroup();
				Log($"Center programs and services loaded. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				LoadCenterStaff();
				Log($"Center staff loaded. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				LoadCenterClientCases();
				Log($"Center client and cases loaded. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				LoadClientLocationIds();
				Log($"Center client move dates loaded. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				LoadCenterFundingDateIds();
				Log($"Center funding date IDs loaded. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				RemoveInvalidClientCaseRecords();
				Log($"Invalid client case records removed. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				RemoveInvalidStaffRecords();
				Log($"Invalid staff records removed. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				RemoveInvalidServiceTypeRecords();
				Log($"Invalid service type records removed. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				CheckServiceRecordData();
				Log($"Validated Service records. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				RemoveInvalidReceivedHoursRecords();
				Log($"Invalid received hours records removed. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				UpdateFundingDateId();
				Log($"Assigned Funding Date ID. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				UpdateTownTownshipCountyId();
				Log($"Assigned Town, Township, County ID. ({PrintElapsedTime(stepStart)})");

				//move (copy) this table out of the way for ImportServiceRecordsGroupProgramDetailClient()
				var myDataTable = _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Copy();
				myDataTable.TableName = TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL;
				_dataset.Tables.Add(myDataTable);

				_importedServiceRecords = true;
			}
		}

		private void ImportServiceRecordsGroupProgramDetail() {
			_importedServiceRecordsGroupProgramDetail = false;
			var stepStart = DateTime.Now;
			if (LoadGroupProgramDetailImportTable()) {
				Log($"Table loaded: {TABLE_PROGRAMDETAIL_IMPORT} ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				RemoveInvalidGroupServiceTypeRecords();
				Log($"Invalid service type records removed. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				CheckServiceRecordDataProgramDetail();
				Log($"Validated Program Detail records. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				UpdateFundingDateIdProgramDetail();
				Log($"Assigned Funding Date ID. ({PrintElapsedTime(stepStart)})");

				_importedServiceRecordsGroupProgramDetail = true;
			}
		}

		private void ImportServiceRecordsGroupProgramDetailStaff() {
			_importedServiceRecordsGroupProgramDetailStaff = false;
			var stepStart = DateTime.Now;
			if (LoadGroupProgramDetailStaffImportTable()) {
				Log($"Table loaded: {TABLE_PROGRAMDETAIL_STAFFS_IMPORT} ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				RemoveInvalidStaffRecordsGroup();
				Log($"Invalid staff records removed. ({PrintElapsedTime(stepStart)})");

				_importedServiceRecordsGroupProgramDetailStaff = true;
			}
		}

		private void ImportServiceRecordsGroupProgramDetailClient() {
			_importedServiceRecordsGroupProgramDetailClient = false;
			var stepStart = DateTime.Now;

			if (LoadServiceDetailOfClientImportTable()) {
				Log($"Table loaded: {TABLE_SERVICEDETAIL_IMPORT} ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				RemoveInvalidGroupClientCaseRecords();
				Log($"Invalid client case records removed. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				CheckGroupServiceRecordData();
				Log($"Validated Service records. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				UpdateFundingDateId();
				Log($"Assigned Funding Date ID. ({PrintElapsedTime(stepStart)})");

				stepStart = DateTime.Now;
				UpdateTownTownshipCountyId();
				Log($"Assigned Town, Township, County ID. ({PrintElapsedTime(stepStart)})");

				_importedServiceRecordsGroupProgramDetailClient = true;
			}
		}
		#endregion loading and validating

		#region saving to sql server
		public void SaveToSqlServer() {
			string mdbName = Path.GetFileName(_mdbPath);
			WriteDoubleBarToLog();
			Log(string.Format("{0}: {1}", "Saving to SQL Server", mdbName));
			WriteSingleBarToLog();

			try {
				var started = DateTime.Now;
				ImportServiceRecordsToSqlServer();
				ImportServiceRecordsToSqlServerGroupProgramDetail();
				ImportServiceRecordsToSqlServerGroupProgramDetailStaff();
				ImportServiceRecordsToSqlServerGroupProgramDetailClient();
				Log($"Valid records saved. ({PrintElapsedTime(started)})"); //KMS DO include this in Wrapper also?
			} catch (Exception e) {
				LogException(e);
				Log($"Services Import failed: unrecognized error occurred while {"Saving to SQL Server".ToLower()} {mdbName}");
				throw;
			}
		}

		private void ImportServiceRecordsToSqlServer() {
			if (!_importedServiceRecords) {
				Log(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL));
				return;
			}
			if (!_dataset.Tables.Contains(TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL)) {
				Log(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL));
				//KMS DO don't throw an error here?  Old system did
				return;
			}
			if (_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL].Rows.Count <= 0) {
				Log(string.Format(MSG_ERR_NO_RECORDS_IN_DATASET_TABLE_TO_IMPORT, TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL));
				return;
			}
			foreach (DataRow each in _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT_INDIVIDUAL].Rows)
				if (each.RowState == DataRowState.Added || each.RowState == DataRowState.Modified || each.RowState == DataRowState.Unchanged)
					if (!UpdateSqlServiceDetailTable(each)) {
						LogExceptionRecord(each, "Record not imported due to SQL Server exception.");
						_invalidImportRows.Enqueue(each);
					} else {
						_importRecordCount++;
					}
		}

		private void ImportServiceRecordsToSqlServerGroupProgramDetail() {
			if (!_importedServiceRecordsGroupProgramDetail) {
				Log(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_PROGRAMDETAIL_IMPORT));
				return;
			}
			if (!_dataset.Tables.Contains(TABLE_PROGRAMDETAIL_IMPORT)) {
				Log(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_PROGRAMDETAIL_IMPORT));
				throw new SystemException(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_PROGRAMDETAIL_IMPORT));
			}
			if (_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows.Count <= 0) {
				Log(string.Format(MSG_ERR_NO_RECORDS_IN_DATASET_TABLE_TO_IMPORT, TABLE_PROGRAMDETAIL_IMPORT));
				return;
			}
			foreach (DataRow each in _dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows)
				if (each.RowState == DataRowState.Added || each.RowState == DataRowState.Modified || each.RowState == DataRowState.Unchanged)
					if (!UpdateSqlProgramDetailTable(each)) {
						LogExceptionRecord(each, "Record not imported due to SQL Server exception.");
						_invalidImportRows.Enqueue(each);
					} else {
						_importRecordCount++;
					}
		}

		private void ImportServiceRecordsToSqlServerGroupProgramDetailStaff() {
			if (!_importedServiceRecordsGroupProgramDetailStaff) {
				Log(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_PROGRAMDETAIL_STAFFS_IMPORT));
				return;
			}
			if (!_dataset.Tables.Contains(TABLE_PROGRAMDETAIL_STAFFS_IMPORT)) {
				Log(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_PROGRAMDETAIL_STAFFS_IMPORT));
				throw new SystemException(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_PROGRAMDETAIL_STAFFS_IMPORT));
			}
			if (_dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT].Rows.Count <= 0) {
				Log(string.Format(MSG_ERR_NO_RECORDS_IN_DATASET_TABLE_TO_IMPORT, TABLE_PROGRAMDETAIL_STAFFS_IMPORT));
				return;
			}
			foreach (DataRow each in _dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT].Rows)
				if (each.RowState == DataRowState.Added || each.RowState == DataRowState.Modified || each.RowState == DataRowState.Unchanged)
					if (!UpdateSqlProgramStaffTable(each)) {
						LogExceptionRecord(each, "Record not imported due to SQL Server exception.");
						_invalidImportRows.Enqueue(each);
					} else {
						_importRecordCount++;
					}
		}

		private void ImportServiceRecordsToSqlServerGroupProgramDetailClient() {
			if (!_importedServiceRecordsGroupProgramDetailClient) {
				Log(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_SERVICEDETAIL_IMPORT));
				return;
			}
			if (!_dataset.Tables.Contains(TABLE_SERVICEDETAIL_IMPORT)) {
				Log(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_SERVICEDETAIL_IMPORT));
				throw new SystemException(string.Format(MSG_ERR_TABLE_NOT_LOADED_IN_DATASET, TABLE_SERVICEDETAIL_IMPORT));
			}
			if (_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows.Count <= 0) {
				Log(string.Format(MSG_ERR_NO_RECORDS_IN_DATASET_TABLE_TO_IMPORT, TABLE_SERVICEDETAIL_IMPORT));
				return;
			}
			foreach (DataRow each in _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows)
				if (each.RowState == DataRowState.Added || each.RowState == DataRowState.Modified || each.RowState == DataRowState.Unchanged)
					if (!UpdateSqlProgramClientTable(each)) {
						LogExceptionRecord(each, "Record not imported due to SQL Server exception.");
						_invalidImportRows.Enqueue(each);
					} else {
						_importRecordCount++;
					}
		}
		#endregion saving to sql server

		#region dumping exceptions
		public void DumpExceptions() {
			string mdbName = Path.GetFileName(_mdbPath);
			WriteDoubleBarToLog();
			Log(string.Format("{0}: {1}", "Dumping Exceptions to CSV", mdbName));
			WriteSingleBarToLog();

			try {
				DumpCsv(TABLE_EXCEPTION_RECORDS);
				DumpCsv(TABLE_EXCEPTION_RECORDS_PGMDETAIL);
				DumpCsv(TABLE_EXCEPTION_RECORDS_STAFF);
			} catch (Exception e) {
				LogException(e);
				Log($"Services Import failed: unrecognized error occurred while {"Dumping Exceptions to CSV".ToLower()} {mdbName}");
				throw;
			}
		}

		private void DumpCsv(string tableName) {
			if (!_dataset.Tables.Contains(tableName)) {
				Log($"No {tableName} to dump.");
				return;
			}

			var started = DateTime.Now;
			string outputFile = Path.Combine(Path.GetDirectoryName(_mdbPath), $"{Path.GetFileNameWithoutExtension(_mdbPath)}_{tableName}.csv");

			if (File.Exists(outputFile)) //KMS DO remove this?
				throw new IOException(string.Format(MSG_ERR_CANNOT_OVERWRITE_EXISTING_FILE, Path.GetFileName(outputFile))); //KMS DO shouldn't contain full file path....check for more logs/errors that contain full path

			using (var csv = new StreamWriter(new FileStream(outputFile, FileMode.CreateNew, FileAccess.Write, FileShare.Read), Encoding.UTF8))
				_dataset.Tables[tableName].WriteCsv(csv);

			Log($"Dumped exceptions to: {Path.GetFileName(outputFile)} ({PrintElapsedTime(started)})");
		}
		#endregion dumping exceptions

		private bool CanApplyForeignKey(DataTable foreignTable, DataColumn primaryKey, DataColumn foreignKey) {
			return CanApplyForeignKey(foreignTable, new[] { primaryKey }, new[] { foreignKey });
		}

		private bool CanApplyForeignKey(DataTable foreignTable, DataColumn[] primaryKey, DataColumn[] foreignKey) {
			ForeignKeyConstraint myFkc = null;
			try {
				myFkc = new ForeignKeyConstraint(primaryKey, foreignKey);
				foreignTable.Constraints.Add(myFkc);
				foreignTable.DataSet.EnforceConstraints = true;
				return true;
			} catch (ArgumentException) {
				return false;
			} finally {
				if (myFkc != null)
					foreignTable.Constraints.Remove(myFkc);
			}
		}

		private void CheckServiceRecordData() {
			foreach (DataRow each in _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows) {
				//try {
				if (each.RowState == DataRowState.Deleted)
					continue;

				var firstcontactDate = GetFirstContactDate((int)each["ClientID"], (int)each["CaseID"]) ?? default(DateTime);

				if (!Convert.IsDBNull(each["agency_ics_id"])) { //Bypass records where agency_ics_id is not null since these are group services and will be processed accordingly
                    _invalidImportRows.Enqueue(each);
				} else if (Convert.IsDBNull(each["LocationID"]) || (int)each["LocationID"] != _centerId) {
					LogExceptionRecord(each, string.Format(MSG_LOCATIONID_MUST_MATCH_CENTERID, _centerId));
					_invalidImportRows.Enqueue(each);
				} else if (Convert.IsDBNull(each["ServiceID"])) {
                    LogExceptionRecord(each, MSG_SERVICEID_REQUIRED);
                    _invalidImportRows.Enqueue(each);
                } else if (ServiceDetailOfClient.AllShelterIds.Contains((int)each["ServiceID"])) {
                    if (!Convert.IsDBNull(each["ServiceDate"])) {
                        LogExceptionRecord(each, MSG_SHELTER_CONTAINS_NON_SHELTER_DATA);
                        _invalidImportRows.Enqueue(each);
                    } else if (Convert.IsDBNull(each["ReceivedHours"])) {
                        LogExceptionRecord(each, MSG_SHELTER_SERVICE_RECEIVEDHOURS_REQUIRES_ZERO);
                        _invalidImportRows.Enqueue(each);
                    } else if (!Convert.IsDBNull(each["ReceivedHours"]) && (double)each["ReceivedHours"] != 0) {
                        LogExceptionRecord(each, MSG_SHELTER_CONTAINS_NON_SHELTER_DATA);
                        _invalidImportRows.Enqueue(each);
                    } else if (!Convert.IsDBNull(each["ShelterBegDate"]) && (DateTime)each["ShelterBegDate"] < firstcontactDate) {
                        LogExceptionRecord(each, MSG_SERVICEDATE_MUST_BE_ON_OR_AFTER_FIRSTCONTACTDATE + " " + firstcontactDate.ToShortDateString());
                        _invalidImportRows.Enqueue(each);
                    } else if (!Convert.IsDBNull(each["ShelterEndDate"]) && (DateTime)each["ShelterEndDate"] < firstcontactDate) {
                        LogExceptionRecord(each, MSG_SERVICEDATE_MUST_BE_ON_OR_AFTER_FIRSTCONTACTDATE + " " + firstcontactDate.ToShortDateString());
                        _invalidImportRows.Enqueue(each);
                    } else if (!Convert.IsDBNull(each["ShelterBegDate"]) && !Convert.IsDBNull(each["ShelterEndDate"]) && ((DateTime)each["ShelterEndDate"] - (DateTime)each["ShelterBegDate"]).Seconds < 0) {
                        LogExceptionRecord(each, MSG_SHELTER_BEGIN_MUST_BE_ON_OR_BEFORE_SHELTER_END);
                        _invalidImportRows.Enqueue(each);
                    } else if (Convert.IsDBNull(each["ShelterBegDate"]) && Convert.IsDBNull(each["ShelterEndDate"]) || Convert.IsDBNull(each["ShelterBegDate"]) && !Convert.IsDBNull(each["ShelterEndDate"])) {
                        LogExceptionRecord(each, MSG_SHELTER_SERVICE_BEGINDATE_REQUIRED);
                        _invalidImportRows.Enqueue(each);
                    }
				} else {
					if (!Convert.IsDBNull(each["ShelterBegDate"]) || !Convert.IsDBNull(each["ShelterEndDate"])) {
						LogExceptionRecord(each, MSG_NONSHELTER_SERVICES_REQUIRE_NULL_SHELTER_DATES);
						_invalidImportRows.Enqueue(each);
					} else if (Convert.IsDBNull(each["ServiceDate"])) {
						LogExceptionRecord(each, MSG_NONSHELTER_SERVICES_REQUIRE_SERVICEDATE);
						_invalidImportRows.Enqueue(each);
                    } else if (Convert.IsDBNull(each["SVID"])) {
                        LogExceptionRecord(each, MSG_NONSHELTER_SERVICES_REQUIRE_SVID);
                        _invalidImportRows.Enqueue(each);
                    } else if ((DateTime)each["ServiceDate"] < firstcontactDate) {
						LogExceptionRecord(each, MSG_SERVICEDATE_MUST_BE_ON_OR_AFTER_FIRSTCONTACTDATE + " " + firstcontactDate.ToShortDateString());
						_invalidImportRows.Enqueue(each);
					} else if (Convert.IsDBNull(each["ReceivedHours"])) {
						LogExceptionRecord(each, MSG_NONSHELTER_SERVICES_REQUIRE_NONZERO_RECEIVED_HOURS);
						_invalidImportRows.Enqueue(each);
					} else if ((double)each["ReceivedHours"] == 0) {
						LogExceptionRecord(each, MSG_NONSHELTER_SERVICES_REQUIRE_NONZERO_RECEIVED_HOURS);
						_invalidImportRows.Enqueue(each);
					}
				}
				//} catch (SystemException ex) {
				//	LogException(ex);
				//	//throw new SystemException("Unhandled Exception in module:" + this, ex); //KMS DO why is this one commented out?
				//}
			}
			RemoveInvalidRows();
		}

		private void CheckGroupServiceRecordData() {
			foreach (DataRow each in _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows) {
				//try
				//{
				if (each.RowState == DataRowState.Deleted)
					continue;

				//KMS DO if this is needed, it is only to throw an exception...
				GetFirstContactDate((int)each["ClientID"], (int)each["CaseID"]);

				if (Convert.IsDBNull(each["agency_ics_id"]))
					_invalidImportRows.Enqueue(each);

				if (Convert.IsDBNull(each["LocationID"]) || (int)each["LocationID"] != _centerId) {
					LogExceptionRecord(each, string.Format(MSG_LOCATIONID_MUST_MATCH_CENTERID, _centerId));
					_invalidImportRows.Enqueue(each);
				}

				//hours validation

				//}
				//catch (SystemException ex)
				//{
				//    LogException(ex);
				//    //throw new SystemException("Unhandled Exception in module:" + this, ex);
				//    throw;
				//}
			}
			RemoveInvalidRows();
		}

		private void CheckServiceRecordDataProgramDetail() {
			foreach (DataRow each in _dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows) {
				// try
				// {
				if (each.RowState == DataRowState.Deleted)
					continue;

				if (Convert.IsDBNull(each["Pdate"])) {
					LogExceptionRecordProgramDetail(each, MSG_PROGRAM_DETAIL_REQUIRES_PDATE);
					_invalidImportRows.Enqueue(each);
					continue;
				}

				if (Convert.IsDBNull(each["NumOfSessions"]) || (int)each["NumOfSessions"] == 0) {
					LogExceptionRecordProgramDetail(each, MSG_PROGRAM_DETAIL_REQUIRES_NUMOFSESSIONS);
					_invalidImportRows.Enqueue(each);
				}
				// }
				// catch (SystemException ex)
				//{
				//    LogException(ex);
				//    //throw new SystemException("Unhandled Exception in module:" + this, ex);
				//    throw;
				//}
			}
			RemoveInvalidRowsProgramDetail();
		}

		private DateTime? GetFirstContactDate(int clientId, int caseId) {
			return (DateTime?)_dataset.Tables[TABLE_CLIENTID_CASEID].Rows.Find(new object[] { clientId, caseId })?["FirstContactDate"];
		}

		#region create exception tables
		private void CreateExceptionRecordsTable() {
			if (_dataset.Tables.Contains(TABLE_EXCEPTION_RECORDS))
				return;

			var table = _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Clone();
			table.TableName = TABLE_EXCEPTION_RECORDS;
			table.Columns.Add(new DataColumn("ExceptionComments"));
			_dataset.Tables.Add(table);
		}

		private void CreateExceptionRecordsTableForStaff() {
			if (_dataset.Tables.Contains(TABLE_EXCEPTION_RECORDS_STAFF))
				return;

			var table = _dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT].Clone();
			table.TableName = TABLE_EXCEPTION_RECORDS_STAFF;
			table.Columns.Add(new DataColumn("ExceptionComments"));
			_dataset.Tables.Add(table);
		}

		private void CreateExceptionRecordsTableForProgramDetail() {
			if (_dataset.Tables.Contains(TABLE_EXCEPTION_RECORDS_PGMDETAIL))
				return;

			var table = _dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Clone();
			table.TableName = TABLE_EXCEPTION_RECORDS_PGMDETAIL;
			table.Columns.Add(new DataColumn("ExceptionComments"));
			_dataset.Tables.Add(table);
		}
		#endregion create exception tables

		private bool IsNewServiceDetailOfClient(DataRow row) {
			//try
			//{
			using (var cmd = _infonet.Sql($"Select count(*) from {TABLE_SQL_SERVICEDETAIL} Where ClientID = @p0 AND CaseID = @p1 AND AgencyRecID = @p2", row["ClientID"], row["CaseID"], row["AgencyRecID"]))
				return Convert.ToInt32(cmd.ExecuteScalar()) == 0;
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private int GetMaxIcsIdFor(int agencyIcsId) {
			using (var cmd = _infonet.Sql($"Select ISNULL(MAX(ICS_ID),0) from {TABLE_SQL_PROGRAMDETAIL} Where Agency_ICS_ID = @p0 AND CenterID = @p1", agencyIcsId, _centerId))
				return Convert.ToInt32(cmd.ExecuteScalar());
		}

		private void LoadCenterFundingDateIds() {
			var table = new DataTable(TABLE_FUNDING_DATE_IDS);
			using (var cmd = _infonet.Sql($"SELECT DISTINCT FundingDate, FundDateID FROM {TABLE_FUNDING_DATE_IDS} WHERE CenterID = @p0 ORDER BY FundingDate", _centerId))
			using (var adapter = new SqlDataAdapter(cmd))
				adapter.Fill(table);
			table.PrimaryKey = new[] { table.Columns["FundingDate"], table.Columns["FundDateID"] };
			_dataset.Tables.Add(table);
		}

		private void LoadClientLocationIds() {
			//try
			//{
			var table = new DataTable(TABLE_TOWNTOWNSHIPCOUNTY);
			using (var cmd = _infonet.Sql($"SELECT DISTINCT ClientID, MoveDate, LocID FROM {TABLE_TOWNTOWNSHIPCOUNTY} WHERE (ClientID IN (SELECT T_Client.ClientID FROM T_Client WHERE (dbo.T_Client.CenterID IN (SELECT ParentCenterID FROM t_center WHERE CenterID = @p0)))) ORDER BY ClientID, MoveDate", _centerId))
			using (var adapter = new SqlDataAdapter(cmd))
				adapter.Fill(table);
			table.PrimaryKey = new[] { table.Columns["ClientID"], table.Columns["MoveDate"] };
			_dataset.Tables.Add(table);
			//}
			//catch (Exception ex)
			//{
			//    //KMS DO can a client have multiple records with same movedate?  If not, i see no reason to keep this try-catch
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void LoadCenterClientCases() {
			var table = new DataTable(TABLE_CLIENTID_CASEID);
			int centerId = _centerId;
			string sql = $"SELECT DISTINCT ClientID, CaseID, FirstContactDate FROM {TABLE_CLIENTID_CASEID} WHERE (ClientID IN (SELECT T_Client.ClientID FROM T_Client WHERE (dbo.T_Client.CenterID IN (SELECT ParentCenterID FROM t_center WHERE CenterID = {centerId} )))) ORDER BY ClientID, CaseID";
			using (var cmd = _infonet.Sql(sql))
			using (var adapter = new SqlDataAdapter(cmd))
				adapter.Fill(table);
			table.PrimaryKey = new[] { table.Columns["ClientID"], table.Columns["CaseID"] };
			_dataset.Tables.Add(table);
		}

		private void LoadCenterProgramsAndServices() {
			using (var cmd = _infonet.StoredProcedure("LOOKUPLIST_GetItems", new SqlParameters {
				["TableName"] = TABLE_PROGRAMS_AND_SERVICES,
				["ProviderID"] = _providerId,
				["Boundfield"] = "CodeID",
				["Displayfield"] = "Description",
				["CenterID"] = _centerId
			}))
			using (var adapter = new SqlDataAdapter(cmd))
				adapter.Fill(_dataset, TABLE_PROGRAMS_AND_SERVICES);
		}

		private void LoadCenterProgramsAndServicesGroup() {
			var table = new DataTable(TABLE_PROGRAMS_AND_SERVICESGROUP);
			using (var cmd = _infonet.Sql($"Select CodeID  from {TABLE_PROGRAMS_AND_SERVICES} where IsCommInst = 1 or IsGroupService = 1"))
			using (var adapter = new SqlDataAdapter(cmd))
				adapter.Fill(table);
			table.PrimaryKey = new[] { table.Columns["CodeID"] };
			_dataset.Tables.Add(table);
		}

		private void LoadCenterStaff() {
			var table = new DataTable(TABLE_STAFF_VOLUNTEER);
			using (var cmd = _infonet.Sql($"Select svid,LastName,FirstName from {TABLE_STAFF_VOLUNTEER} where CenterID = @p0", _centerId))
			using (var adapter = new SqlDataAdapter(cmd))
				adapter.Fill(table);
			table.PrimaryKey = new[] { table.Columns["SVID"] };
			_dataset.Tables.Add(table);
		}

		#region private
		private bool IsValidServiceDetailOfClientTable() {
			//try
			// {
			if (_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns.Count != _ImportTableDataColumns.Length) {
				Log($"ERROR: Table has incorrect number of fields: {TABLE_SERVICEDETAIL_IMPORT}.");
				return false;
			}

			bool result = true;
			foreach (DataColumn eachFound in _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns) {
				bool isColumnNameFound = false;
				foreach (var eachExpected in _ImportTableDataColumns) {
					if (!string.Equals(eachExpected.ColumnName, eachFound.ColumnName, StringComparison.OrdinalIgnoreCase))
						continue;
					isColumnNameFound = true;
					if (eachExpected.DataType.FullName == eachFound.DataType.FullName)
						break;
					Log($"ERROR: Column {eachFound.ColumnName} in table {TABLE_SERVICEDETAIL_IMPORT} has type {eachFound.DataType.Name} but expected {eachExpected.DataType.Name}.");
					result = false;
					break;
				}

				if (!isColumnNameFound) {
					Log($"ERROR: Column: {eachFound.ColumnName} does not exist in table {TABLE_SERVICEDETAIL_IMPORT}");
					result = false;
				}
			}
			return result;
			//}
			//catch
			//{
			//    return false;
			//}
		}

		private bool IsValidTableProgramDetail() {
			//try
			//{
			if (_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Columns.Count != _ImportTableDataColumnsPgmDetail.Length) {
				Log($"ERROR: Table has incorrect number of fields: {TABLE_PROGRAMDETAIL_IMPORT}.");
				return false;
			}
			bool result = true;
			foreach (DataColumn eachFound in _dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Columns) {
				bool isColumnNameFound = false;
				foreach (var eachExpected in _ImportTableDataColumnsPgmDetail) {
					if (!string.Equals(eachExpected.ColumnName, eachFound.ColumnName, StringComparison.OrdinalIgnoreCase))
						continue;
					isColumnNameFound = true;
					if (eachExpected.DataType.FullName == eachFound.DataType.FullName)
						break;
					Log($"ERROR: Column {eachFound.ColumnName} in table {TABLE_PROGRAMDETAIL_IMPORT} has type {eachFound.DataType.Name} but expected {eachExpected.DataType.Name}.");
					result = false;
					break;
				}

				if (!isColumnNameFound) {
					Log($"ERROR: Column: {eachFound.ColumnName} does not exist in table {TABLE_PROGRAMDETAIL_IMPORT}");
					result = false;
				}
			}
			return result;
			//}
			//catch
			//{
			//    return false;
			//}
		}

		private bool IsValidTableProgramDetailStaff() {
			//try
			//{
			if (_dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT].Columns.Count != _ImportTableDataColumnsPgmStaffs.Length) {
				Log($"ERROR: Table has incorrect number of fields: {TABLE_PROGRAMDETAIL_STAFFS_IMPORT}.");
				return false;
			}
			bool result = true;
			foreach (DataColumn eachFound in _dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT].Columns) {
				bool isColumnNameFound = false;

				foreach (var eachExpected in _ImportTableDataColumnsPgmStaffs) {
					if (!string.Equals(eachExpected.ColumnName, eachFound.ColumnName, StringComparison.OrdinalIgnoreCase))
						continue;
					isColumnNameFound = true;
					if (eachExpected.DataType.FullName == eachFound.DataType.FullName)
						break;
					Log($"ERROR: Column {eachFound.ColumnName} in table {TABLE_PROGRAMDETAIL_STAFFS_IMPORT} has type {eachFound.DataType.Name} but expected {eachExpected.DataType.Name}.");
					result = false;
					break;
				}

				if (!isColumnNameFound) {
					Log($"ERROR: Column: {eachFound.ColumnName} does not exist in table {TABLE_PROGRAMDETAIL_STAFFS_IMPORT}");
					result = false;
				}
			}
			return result;
			//}
			//catch
			//{
			//    return false;
			//}
		}

		private bool LoadServiceDetailOfClientImportTable() {
			DataTable table = null;
			DataColumn[] primaryKeys = null;
			try {
				if (_dataset.Tables.Contains(TABLE_SERVICEDETAIL_IMPORT))
					_dataset.Tables.Remove(TABLE_SERVICEDETAIL_IMPORT);

				table = _dataset.GetTable(TABLE_SERVICEDETAIL_IMPORT);

				if (!IsValidServiceDetailOfClientTable())
					throw new Exception(string.Format(MSG_ERR_INVALID_IMPORT_TABLE_SCHEMA, TABLE_SERVICEDETAIL_IMPORT));
				RemoveDuplicateAgencyRecIds();
				table.PrimaryKey = primaryKeys = new[] { table.Columns["ClientID"], table.Columns["CaseID"], table.Columns["AgencyRecID"] };
				return true;
			} catch (ArgumentException) {
				if (table != null && primaryKeys != null)
					table.PrimaryKey = primaryKeys;
				return false;
			}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
			finally {
				_dataset.OleDbConnection.Close(); //KMS DO why?
			}
		}

		private bool LoadGroupProgramDetailImportTable() {
			DataTable table = null;
			DataColumn[] primaryKeys = null;
			try {
				if (_dataset.Tables.Contains(TABLE_PROGRAMDETAIL_IMPORT))
					_dataset.Tables.Remove(TABLE_PROGRAMDETAIL_IMPORT);

				table = _dataset.GetTable(TABLE_PROGRAMDETAIL_IMPORT);

				if (!IsValidTableProgramDetail())
					throw new Exception(string.Format(MSG_ERR_INVALID_IMPORT_TABLE_SCHEMA, TABLE_PROGRAMDETAIL_IMPORT));
				RemoveDuplicateAgencyRecIdsProgramDetail();
				table.PrimaryKey = primaryKeys = new[] { table.Columns["Agency_ICS_ID"] };
				return true;
			} catch (ArgumentException) {
				if (table != null && primaryKeys != null)
					table.PrimaryKey = primaryKeys;
				return false;
			}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
			finally {
				_dataset.OleDbConnection.Close();
			}
		}

		private bool LoadGroupProgramDetailStaffImportTable() {
			DataTable table = null;
			DataColumn[] primaryKeys = null;
			try {
				if (_dataset.Tables.Contains(TABLE_PROGRAMDETAIL_STAFFS_IMPORT))
					_dataset.Tables.Remove(TABLE_PROGRAMDETAIL_STAFFS_IMPORT);

				table = _dataset.GetTable(TABLE_PROGRAMDETAIL_STAFFS_IMPORT);

				if (!IsValidTableProgramDetailStaff())
					throw new Exception(string.Format(MSG_ERR_INVALID_IMPORT_TABLE_SCHEMA, TABLE_PROGRAMDETAIL_STAFFS_IMPORT));

				table.PrimaryKey = primaryKeys = new[] { table.Columns["Agency_ICS_ID"], table.Columns["SVID"] };
				return true;
			} catch (ArgumentException) {
				RemoveDuplicateAgencyRecIdsProgramDetail();
				if (table != null && primaryKeys != null)
					table.PrimaryKey = primaryKeys;
				return false;
			}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
			finally {
				_dataset.OleDbConnection.Close();
			}
		}

		private void RemoveDuplicateAgencyRecIds() {
			var recIdList = new List<object>();
			var dupRecIdList = new Queue();
			int i;

			for (i = 0; i <= _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows.Count - 1; i++)
				if (recIdList.Contains(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows[i]["AgencyRecID"])) {
					if (!dupRecIdList.Contains(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows[i]["AgencyRecID"]))
						dupRecIdList.Enqueue(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows[i]["AgencyRecID"]);
				} else {
					recIdList.Add(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows[i]["AgencyRecID"]);
				}

			var myDataView = new DataView(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT]);
			while (dupRecIdList.Count > 0) {
				try {
					int myRecId = (int)dupRecIdList.Dequeue();
					myDataView.RowFilter = "AgencyRecId = " + myRecId;
				} catch (InvalidCastException) {
					myDataView.RowFilter = "AgencyRecID is Null";
				}

				while (myDataView.Count > 0) {
					//try
					//{
					LogExceptionRecord(myDataView[0].Row, "Multiple Records with same AgencyRecordID");
					myDataView.Delete(0);
					//}
					//catch (Exception ex)
					//{
					//    LogException(ex);
					//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
					//    throw;
					//}
				}
			}
		}

		private void RemoveDuplicateAgencyRecIdsProgramDetail() {
			var recIdList = new List<object>(); //KMS DO do better
			var dupRecIdList = new Queue();
			int i;

			for (i = 0; i <= _dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows.Count - 1; i++)
				if (recIdList.Contains(_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows[i]["Agency_ICS_ID"])) {
					if (!dupRecIdList.Contains(_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows[i]["Agency_ICS_ID"]))
						dupRecIdList.Enqueue(_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows[i]["Agency_ICS_ID"]);
				} else {
					recIdList.Add(_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows[i]["Agency_ICS_ID"]);
				}

			var myDataView = new DataView(_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT]);

			while (dupRecIdList.Count > 0) {
				try {
					int myRecId = (int)dupRecIdList.Dequeue();
					myDataView.RowFilter = "Agency_ICS_ID = " + myRecId;
				} catch (InvalidCastException) {
					myDataView.RowFilter = "Agency_ICS_ID is Null";
				}

				while (myDataView.Count > 0) {
					//try
					//{
					LogExceptionRecord(myDataView[0].Row, "Multiple Records with same AgencyRecordID");
					myDataView.Delete(0);
					//}
					//catch (Exception ex)
					//{
					//    LogException(ex);
					//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
					//    throw;
					//}
				}
			}
		}

		private void RemoveClientCaseForeignKeyViolators() {
			var validClientCases = new List<string>(); //KMS DO convert this to HashSet

			//try
			//{
			foreach (DataRow each in _dataset.Tables[TABLE_CLIENTID_CASEID].Rows)
				validClientCases.Add(each["ClientID"] + "-" + each["CaseID"]);

			foreach (DataRow each in _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows)
				try {
					if (!validClientCases.Contains(each["ClientID"] + "-" + each["CaseID"])) {
						LogExceptionRecord(each, "ClientID-CaseID combination do not exist in Infonet Database");
						_invalidImportRows.Enqueue(each);
					}
				} catch (DeletedRowInaccessibleException) { }
			RemoveInvalidRows();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveServiceIdForeignKeyViolators() {
			var valildServiceIDs = new List<object>();

			//try
			//{
			foreach (DataRow each in _dataset.Tables[TABLE_PROGRAMS_AND_SERVICES].Rows)
				valildServiceIDs.Add(each["CodeID"]);

			foreach (DataRow each in _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows)
				try {
					if (!valildServiceIDs.Contains(each["ServiceID"])) {
						LogExceptionRecord(each, "ServiceID is not valid for this center.");
						_invalidImportRows.Enqueue(each);
					}
				} catch (DeletedRowInaccessibleException) { }
			RemoveInvalidRows();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveServiceIdForeignKeyViolatorsGroup() {
			var valildServiceIDs = new List<object>();

			//try
			//{
			foreach (DataRow each in _dataset.Tables[TABLE_PROGRAMS_AND_SERVICESGROUP].Rows)
				valildServiceIDs.Add(each["CodeID"]);

			foreach (DataRow each in _dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows)
				try {
					if (!valildServiceIDs.Contains(each["ProgramID"])) {
						LogExceptionRecordProgramDetail(each, "ProgramID is not a valid group service.");
						_invalidImportRows.Enqueue(each);
					}
				} catch (DeletedRowInaccessibleException) { }
			RemoveInvalidRowsProgramDetail();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveStaffIdForeignKeyViolators() {
			var validStaffIds = new List<object>();

			//try
			//{
			foreach (DataRow each in _dataset.Tables[TABLE_STAFF_VOLUNTEER].Rows)
				validStaffIds.Add(each["SVID"]);
			foreach (DataRow each in _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows)
				try {
					if (!validStaffIds.Contains(each["SVID"])) {
						LogExceptionRecord(each, "StaffID does not exist in Infonet Database");
						_invalidImportRows.Enqueue(each);
					}
				} catch (DeletedRowInaccessibleException) { }

			RemoveInvalidRows(); //KMS DO does this make sense?
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveReceivedHoursForeignKeyViolators() {
			foreach (DataRow each in _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows)
				try {
                    var eachReceivedHours = each["ReceivedHours"];
                    var ics_id = each["Agency_ics_id"];
                    bool isQuarter = Convert.ToDouble(eachReceivedHours) * 4 == Math.Floor(Convert.ToDouble(eachReceivedHours) * 4);
                    bool isWithinRange = true;

                    if (!Convert.IsDBNull(ics_id)) //Group Detail
                        isWithinRange = Convert.ToDouble(eachReceivedHours) > 0 && Convert.ToDouble(eachReceivedHours) <= 999;
                    else if (!ServiceDetailOfClient.AllShelterIds.Contains((int)each["ServiceID"]))
                        isWithinRange = Convert.ToDouble(eachReceivedHours) > 0 && Convert.ToDouble(eachReceivedHours) <= 100; //Service detail

                    if (!isQuarter) {
                        LogExceptionRecord(each, "ReceivedHours is not a quarter hour increment.");
                        _invalidImportRows.Enqueue(each);
                    }

                    if (!isWithinRange && !Convert.IsDBNull(ics_id)) {
                        LogExceptionRecord(each, "ReceivedHours is not between 0 and 999."); //Group Detail
                        _invalidImportRows.Enqueue(each);
                    }

                    if (!isWithinRange && Convert.IsDBNull(ics_id)) {
                        LogExceptionRecord(each, "ReceivedHours is not between 0 and 100."); //Service detail
                        _invalidImportRows.Enqueue(each);
                    }
                } catch (DeletedRowInaccessibleException) { }

			RemoveInvalidRows();
		}

		private void RemoveStaffIdForeignKeyViolatorsGroup() {
			var validStaffIds = new List<object>();

			//try
			//{
			foreach (DataRow each in _dataset.Tables[TABLE_STAFF_VOLUNTEER].Rows)
				validStaffIds.Add(each["SVID"]);

			foreach (DataRow each in _dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT].Rows)
				try {
					if (!validStaffIds.Contains(each["SVID"])) {
						LogExceptionRecordStaff(each, "StaffID does not exist in Infonet Database for this center.");
						// Record row to be deleted after Iteration completes
						_invalidImportRows.Enqueue(each);
					}
				} catch (DeletedRowInaccessibleException) { }

			RemoveInvalidRowsStaff();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveInvalidClientCaseRecords() {
			//try
			//{
			DataColumn[] primaryKey = { _dataset.Tables[TABLE_CLIENTID_CASEID].Columns["ClientID"], _dataset.Tables[TABLE_CLIENTID_CASEID].Columns["CaseID"] };
			DataColumn[] foreignKey = { _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns["ClientID"], _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns["CaseID"] };
			while (!CanApplyForeignKey(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT], primaryKey, foreignKey))
				RemoveClientCaseForeignKeyViolators();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveInvalidGroupClientCaseRecords() {
			//try {
			DataColumn[] primaryKey = { _dataset.Tables[TABLE_CLIENTID_CASEID].Columns["ClientID"], _dataset.Tables[TABLE_CLIENTID_CASEID].Columns["CaseID"] };
			DataColumn[] foreignKey = { _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns["ClientID"], _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns["CaseID"] };
			while (!CanApplyForeignKey(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT], primaryKey, foreignKey))
				RemoveClientCaseForeignKeyViolators();
			//} catch (Exception ex) {
			//	LogException(ex);
			//throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveInvalidRows() {
			try {
				while (_invalidImportRows.Count > 0) {
					var myDataRow = (DataRow)_invalidImportRows.Dequeue();
					_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Rows.Remove(myDataRow);
				}
			}
			//catch (SystemException ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
			finally {
				_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT]?.AcceptChanges();
			}
		}

		private void RemoveInvalidRowsStaff() {
			try {
				while (_invalidImportRows.Count > 0) {
					var myDataRow = (DataRow)_invalidImportRows.Dequeue();
					_dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT].Rows.Remove(myDataRow);
				}
			}
			//catch (SystemException ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
			finally {
				_dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT]?.AcceptChanges();
			}
		}

		private void RemoveInvalidRowsProgramDetail() {
			try {
				while (_invalidImportRows.Count > 0) {
					var myDataRow = (DataRow)_invalidImportRows.Dequeue();
					_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Rows.Remove(myDataRow);
				}
			}
			//catch (SystemException ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
			finally {
				_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT]?.AcceptChanges();
			}
		}

		private void RemoveInvalidServiceTypeRecords() {
			//try
			//{
			var primaryKey = _dataset.Tables[TABLE_PROGRAMS_AND_SERVICES].Columns["CodeID"];
			var foreignKey = _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns["ServiceID"];
			while (!CanApplyForeignKey(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT], primaryKey, foreignKey))
				RemoveServiceIdForeignKeyViolators();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveInvalidReceivedHoursRecords() {
			//try
			//{
			//var primaryKey = _dataset.Tables[TABLE_SQL_SERVICEDETAIL].Columns["ReceivedHours"];
			//var foreignKey = _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns["ReceivedHours"];
			//while (!CanApplyForeignKey(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT], primaryKey, foreignKey))
			RemoveReceivedHoursForeignKeyViolators();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveInvalidGroupServiceTypeRecords() {
			//try
			//{
			var primaryKey = _dataset.Tables[TABLE_PROGRAMS_AND_SERVICESGROUP].Columns["CodeID"];
			var foreignKey = _dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Columns["ProgramID"];
			while (!CanApplyForeignKey(_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT], primaryKey, foreignKey))
				RemoveServiceIdForeignKeyViolatorsGroup();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveInvalidStaffRecords() {
			//try
			//{
			var primaryKey = _dataset.Tables[TABLE_STAFF_VOLUNTEER].Columns["SVID"];
			var foreignKey = _dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns["SVID"];
			while (!CanApplyForeignKey(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT], primaryKey, foreignKey))
				RemoveStaffIdForeignKeyViolators();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void RemoveInvalidStaffRecordsGroup() {
			//try
			//{
			var primaryKey = _dataset.Tables[TABLE_STAFF_VOLUNTEER].Columns["SVID"];
			var foreignKey = _dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT].Columns["SVID"];
			while (!CanApplyForeignKey(_dataset.Tables[TABLE_PROGRAMDETAIL_STAFFS_IMPORT], primaryKey, foreignKey))
				RemoveStaffIdForeignKeyViolatorsGroup();
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void LogExceptionRecord(DataRow sourceDataRow, string exceptionMessage) {
			//try
			//{
			CreateExceptionRecordsTable();

			var myDataRow = _dataset.Tables[TABLE_EXCEPTION_RECORDS].NewRow();

			var myArrayList = new List<object>();

			foreach (object each in sourceDataRow.ItemArray)
				myArrayList.Add(each);

			myArrayList.Add(exceptionMessage);

			myDataRow.ItemArray = myArrayList.ToArray();

			try {
				_dataset.Tables[TABLE_EXCEPTION_RECORDS].Rows.Add(myDataRow);
			} catch (Exception e) {
				//swallow duplicate inserts
			}

			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void LogExceptionRecordStaff(DataRow sourceDataRow, string exceptionMessage) {
			var myArrayList = new List<object>();

			//try
			//{
			CreateExceptionRecordsTableForStaff();

			var myDataRow = _dataset.Tables[TABLE_EXCEPTION_RECORDS_STAFF].NewRow();

			myArrayList.Clear();

			var mySystemArray = sourceDataRow.ItemArray;
			for (int i = 0; i <= mySystemArray.Length - 1; i++)
				myArrayList.Add(mySystemArray[i].ToString());

			myArrayList.Add(exceptionMessage);

			myDataRow.ItemArray = myArrayList.ToArray();
			_dataset.Tables[TABLE_EXCEPTION_RECORDS_STAFF].Rows.Add(myDataRow);
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    throw;
			//}
		}

		private void LogExceptionRecordProgramDetail(DataRow sourceDataRow, string exceptionMessage) {
			//try
			//{
			CreateExceptionRecordsTableForProgramDetail();

			var myDataRow = _dataset.Tables[TABLE_EXCEPTION_RECORDS_PGMDETAIL].NewRow();

			var myArrayList = new List<object>();
			var mySystemArray = sourceDataRow.ItemArray;
			for (int i = 0; i <= mySystemArray.Length - 1; i++)
				myArrayList.Add(mySystemArray[i]);

			myArrayList.Add(exceptionMessage);

			myDataRow.ItemArray = myArrayList.ToArray();
			_dataset.Tables[TABLE_EXCEPTION_RECORDS_PGMDETAIL].Rows.Add(myDataRow);
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void UpdateFundingDateId() {
			//try
			//{
			if (!_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns.Contains("FundDateID"))
				_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns.Add(new DataColumn("FundDateID", typeof(int)));

			var fundingDatesDataView = new DataView(_dataset.Tables[TABLE_FUNDING_DATE_IDS]) { Sort = "FundingDate DESC" };
			var servicesDataView = new DataView(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT]);
			for (int i = 0; i <= fundingDatesDataView.Count - 1; i++) {
				var fundingDateDataRow = fundingDatesDataView[i].Row;
				servicesDataView.RowFilter = "FundDateID is null " + " AND (ServiceDate >= '" + fundingDateDataRow["FundingDate"] + "' " + " OR ShelterBegDate >= '" + fundingDateDataRow["FundingDate"] + "') ";
				while (servicesDataView.Count > 0)
					servicesDataView[0].Row["FundDateID"] = fundingDateDataRow["FundDateID"];
			}
			servicesDataView.RowFilter = ""; //KMS DO remove
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private void UpdateFundingDateIdProgramDetail() {
			//try
			//{
			if (!_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Columns.Contains("FundDateID"))
				_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT].Columns.Add(new DataColumn("FundDateID", typeof(int)));

			var fundingDatesDataView = new DataView(_dataset.Tables[TABLE_FUNDING_DATE_IDS]) { Sort = "FundingDate DESC" };
			var servicesDataView = new DataView(_dataset.Tables[TABLE_PROGRAMDETAIL_IMPORT]);
			for (int i = 0; i <= fundingDatesDataView.Count - 1; i++) {
				var fundingDateDataRow = fundingDatesDataView[i].Row;
				servicesDataView.RowFilter = "FundDateID is null " + " AND (PDate >= '" + fundingDateDataRow["FundingDate"] + "') ";
				while (servicesDataView.Count > 0)
					servicesDataView[0].Row["FundDateID"] = fundingDateDataRow["FundDateID"];
			}
			servicesDataView.RowFilter = ""; //KMS DO remove
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}

		private bool UpdateSqlServiceDetailTable(DataRow myDataRow) {
			bool isAdd = false;

			//try
			//{
			var myArrayList = new List<object>();
			if (myDataRow.Table.PrimaryKey.Length > 0) {
				var myArray = myDataRow.Table.PrimaryKey;
				for (int i = 0; i <= myArray.Length - 1; i++)
					myArrayList.Add(myArray[i]);
			}
			if (myArrayList.Count <= 0) {
				Log(string.Format(MSG_ERR_NO_PRIMARY_KEYS_DEFINED_FOR_TABLE, myDataRow.Table.TableName));
				throw new SystemException(string.Format(MSG_ERR_NO_PRIMARY_KEYS_DEFINED_FOR_TABLE, myDataRow.Table.TableName));
			}

			var mySqL = new StringBuilder();

			if (IsNewServiceDetailOfClient(myDataRow)) {
				isAdd = true;
				mySqL.Append("INSERT INTO ");
				mySqL.Append(TABLE_SQL_SERVICEDETAIL);
				mySqL.Append(" (");

				foreach (DataColumn each in myDataRow.Table.Columns)
					if (each.ColumnName.ToUpper() != "AGENCY_ICS_ID") {
						mySqL.Append(each.ColumnName);
						if (each.Ordinal < myDataRow.Table.Columns.Count - 1)
							mySqL.Append(",");
					}
				mySqL.Append(")");
				mySqL.Append("VALUES (");
				foreach (DataColumn each in myDataRow.Table.Columns)
					if (each.ColumnName.ToUpper() != "AGENCY_ICS_ID") {
						if (Convert.IsDBNull(myDataRow[each.ColumnName]))
							mySqL.Append("NULL");
						else
							switch (each.DataType.ToString().ToUpper()) {
								case "SYSTEM.DATETIME":
									mySqL.Append("'");
									mySqL.Append(myDataRow[each.ColumnName]);
									mySqL.Append("'");
									break;

								default:
									mySqL.Append(myDataRow[each.ColumnName]);
									break;
							}
						if (each.Ordinal < myDataRow.Table.Columns.Count - 1)
							mySqL.Append(",");
					}
				mySqL.Append(")");
			} else {
				mySqL.Append("Update ");
				mySqL.Append(TABLE_SQL_SERVICEDETAIL);
				mySqL.Append(" ");
				mySqL.Append("Set ");
				foreach (DataColumn each in myDataRow.Table.Columns)
					if (!myArrayList.Contains(each)) {
						if (each.ColumnName.ToUpper() == "AGENCY_ICS_ID")
							each.ColumnName = "ICS_ID";
						mySqL.Append(each.ColumnName);
						mySqL.Append("=");

						if (Convert.IsDBNull(myDataRow[each.ColumnName]))
							mySqL.Append("NULL");
						else
							switch (each.DataType.ToString().ToUpper()) {
								case "SYSTEM.DATETIME":
									mySqL.Append("'");
									mySqL.Append(myDataRow[each.ColumnName]);
									mySqL.Append("'");
									break;

								default:
									mySqL.Append(myDataRow[each.ColumnName]);
									break;
							}
						if (each.Ordinal < myDataRow.Table.Columns.Count - 1)
							mySqL.Append(",");
					}

				if (mySqL.ToString().EndsWith(","))
					mySqL.Length = mySqL.Length - 1;
				mySqL.Append(" Where ");
				foreach (DataColumn each in myArrayList) {
					mySqL.Append(each.ColumnName);
					mySqL.Append("=");

					if (Convert.IsDBNull(myDataRow[each.ColumnName]))
						mySqL.Append("NULL");
					else
						switch (each.DataType.ToString().ToUpper()) {
							case "SYSTEM.DATETIME":
								mySqL.Append("'");
								mySqL.Append(myDataRow[each.ColumnName]);
								mySqL.Append("'");
								break;

							default:
								mySqL.Append(myDataRow[each.ColumnName]);
								break;
						}

					if (each.Ordinal < myDataRow.Table.Columns.Count)
						mySqL.Append(" AND ");
				}

				if (mySqL.ToString().EndsWith(" AND "))
					mySqL.Length = mySqL.Length - 5;
			}

			using (var mySqlCommand = _infonet.Sql(mySqL.ToString()))
				mySqlCommand.ExecuteNonQuery(); //KMS DO check return value?

			if (isAdd)
				_addRecordCount++;
			else
				_updateRecordCount++;
			return true;
			//}
			//catch (SystemException ex)
			//{
			//    LogException(ex);
			//    //return false;
			//    throw;
			//}
		}

		private bool UpdateSqlProgramDetailTable(DataRow myDataRow) {
			bool isAdd = false;

			//try
			//{
			//KMS DO is there a point to this?
			var myArrayList = new List<object>();
			if (myDataRow.Table.PrimaryKey.Length > 0) {
				var myArray = myDataRow.Table.PrimaryKey;
				for (int i = 0; i <= myArray.Length - 1; i++)
					myArrayList.Add(myArray[i]);
			}
			if (myArrayList.Count <= 0) {
				Log(string.Format(MSG_ERR_NO_PRIMARY_KEYS_DEFINED_FOR_TABLE, myDataRow.Table.TableName));
				throw new SystemException(string.Format(MSG_ERR_NO_PRIMARY_KEYS_DEFINED_FOR_TABLE, myDataRow.Table.TableName));
			}

			int idExistNum = GetMaxIcsIdFor((int)myDataRow["agency_ics_id"]);
			if (idExistNum == 0) {
				isAdd = true;
				using (var mySqlCommand = _infonet.Sql($"INSERT INTO {TABLE_SQL_PROGRAMDETAIL} (CenterID, ProgramID, NumOfSession, Hours, ParticipantsNum, PDate, ChildSpecific, Agency_ICS_ID, FundDateID) VALUES (@centerId, @programId, @numOfSessions, @hours, @participantsNum, @pDate, 0, @agencyIcsId, @fundDateId); Select SCOPE_IDENTITY();", new SqlParameters {
					["centerId"] = _centerId,
					["programId"] = myDataRow["ProgramID"],
					["numOfSessions"] = myDataRow["NumOfSessions"],
					["hours"] = myDataRow["Hours"],
					["participantsNum"] = myDataRow["ParticipantsNum"],
					["pDate"] = myDataRow["PDate"],
					["agencyIcsId"] = myDataRow["Agency_ICS_ID"],
					["fundDateId"] = myDataRow["FundDateID"]
				}))
					idExistNum = Convert.ToInt32(mySqlCommand.ExecuteScalar());
			} else {
				using (var mySqlCommand = _infonet.Sql($"Update {TABLE_SQL_PROGRAMDETAIL} Set CenterID = @centerId, ProgramID = @programId, NumOfSession = @numOfSessions, Hours = @hours, ParticipantsNum = @participantsNum, PDate = @pDate, FundDateID = @fundDateId, RevisionStamp = @revisionStamp WHERE ICS_ID = @icsId", new SqlParameters {
					["centerId"] = _centerId,
					["programId"] = myDataRow["ProgramID"],
					["numOfSessions"] = myDataRow["NumOfSessions"],
					["hours"] = myDataRow["Hours"],
					["participantsNum"] = myDataRow["ParticipantsNum"],
					["pDate"] = myDataRow["PDate"],
					["fundDateId"] = myDataRow["FundDateID"],
					["revisionStamp"] = DateTime.Now,
					["icsId"] = idExistNum
				}))
					mySqlCommand.ExecuteNonQuery();
			}

			if (isAdd)
				_addRecordCount++;
			else
				_updateRecordCount++;

			_icsIdTable.Add((int)myDataRow.GetInt32("Agency_ICS_ID"), new ProgramDetailSummary {
				IcsId = idExistNum,
				IsAdd = isAdd,
				ProgramId = (int)ConvertNull.ToInt32(myDataRow["ProgramID"]),
				ServiceDate = (DateTime)ConvertNull.ToDateTime(myDataRow["PDate"]),
				FundDateId = (int)ConvertNull.ToInt32(myDataRow["FundDateID"])
			});
			return true;
			//}
			//catch (SystemException ex)
			//{
			//    LogException(ex);
			//    //return false;
			//    throw;
			//}
		}

		private bool UpdateSqlProgramStaffTable(DataRow myDataRow) {
			bool isAdd = false;

			//try
			//{
			var myArrayList = new List<object>();
			if (myDataRow.Table.PrimaryKey.Length > 0) {
				var myArray = myDataRow.Table.PrimaryKey;
				for (int i = 0; i <= myArray.Length - 1; i++)
					myArrayList.Add(myArray[i]);
			}
			if (myArrayList.Count <= 0) {
				Log(string.Format(MSG_ERR_NO_PRIMARY_KEYS_DEFINED_FOR_TABLE, myDataRow.Table.TableName));
				throw new SystemException(string.Format(MSG_ERR_NO_PRIMARY_KEYS_DEFINED_FOR_TABLE, myDataRow.Table.TableName));
			}

			ProgramDetailSummary programDetail;
			if (_icsIdTable.TryGetValue((int)myDataRow.GetInt32("Agency_ICS_ID"), out programDetail))
				if (programDetail.IsAdd) {
					isAdd = true;
					using (var cmd = _infonet.Sql($"INSERT INTO {TABLE_SQL_PROGRAMDETAIL_STAFFS} (ICS_ID, SVID, ConductHours, HoursPrep, HoursTravel) VALUES (@icsId, @svId, @conductHours, @hoursPrep, @hoursTravel)",
						new SqlParameters {
							["icsId"] = programDetail.IcsId,
							["svId"] = myDataRow["SVID"],
							["conductHours"] = myDataRow["HoursOfConduct"],
							["hoursPrep"] = myDataRow["HoursPrepare"],
							["hoursTravel"] = myDataRow["HoursTravel"]
						}))
						cmd.ExecuteNonQuery();
				} else {
					try {
						if (ExistsInProgramDetailStaffs(programDetail.IcsId, (int)myDataRow["SVID"])) {
							using (var cmd = _infonet.Sql($"Update {TABLE_SQL_PROGRAMDETAIL_STAFFS} Set ConductHours = @conductHours, HoursPrep = @hoursPrep, HoursTravel = @hoursTravel, RevisionStamp = @revisionStamp WHERE ICS_ID = @icsId AND SVID = @svId",
								new SqlParameters {
									["conductHours"] = myDataRow["HoursOfConduct"],
									["hoursPrep"] = myDataRow["HoursPrepare"],
									["hoursTravel"] = myDataRow["HoursTravel"],
									["revisionStamp"] = DateTime.Now,
									["icsId"] = programDetail.IcsId,
									["svId"] = myDataRow["SVID"]
								}))
								cmd.ExecuteNonQuery();
						} else {
							isAdd = true;
							using (var cmd = _infonet.Sql($"INSERT INTO {TABLE_SQL_PROGRAMDETAIL_STAFFS} (ICS_ID, SVID, ConductHours, HoursPrep, HoursTravel) VALUES (@icsId, @svId, @conductHours, @hoursPrep, @hoursTravel)",
								new SqlParameters {
									["icsId"] = programDetail.IcsId,
									["svId"] = myDataRow["SVID"],
									["conductHours"] = myDataRow["HoursOfConduct"],
									["hoursPrep"] = myDataRow["HoursPrepare"],
									["hoursTravel"] = myDataRow["HoursTravel"]
								}))
								cmd.ExecuteNonQuery();
						}
					} catch (SystemException ex) {
						LogException(ex);
						//KMS DO again...just ignore this and continue???
					}
				}

			if (isAdd)
				_addRecordCount++;
			else
				_updateRecordCount++;
			return true;
			//}
			//catch (SystemException ex)
			//{
			//    LogException(ex);
			//    //return false;
			//    throw;
			//}
		}

		private bool UpdateSqlProgramClientTable(DataRow myDataRow) {
			bool isAdd = false;

			//try
			//{
			var myArrayList = new List<object>();
			if (myDataRow.Table.PrimaryKey.Length > 0) {
				var myArray = myDataRow.Table.PrimaryKey;
				for (int i = 0; i <= myArray.Length - 1; i++)
					myArrayList.Add(myArray[i]);
			}
			if (myArrayList.Count <= 0) {
				Log(string.Format(MSG_ERR_NO_PRIMARY_KEYS_DEFINED_FOR_TABLE, myDataRow.Table.TableName));
				throw new SystemException(string.Format(MSG_ERR_NO_PRIMARY_KEYS_DEFINED_FOR_TABLE, myDataRow.Table.TableName));
			}

			ProgramDetailSummary programDetail;
			if (_icsIdTable.TryGetValue((int)myDataRow.GetInt32("Agency_ICS_ID"), out programDetail))
				if (programDetail.IsAdd) {
					isAdd = true;
					using (var cmd = _infonet.Sql($"INSERT INTO {TABLE_SQL_SERVICEDETAIL} (ClientID, CaseID, ServiceID, LocationID, ServiceDate, ReceivedHours, CityTownTownshpID, FundDateID, AgencyRecID, ICS_ID) VALUES (@clientId, @caseId, @serviceId, @locationId, @serviceDate, @receivedHours, @cityTownTownshipId, @fundDateId, @agencyRecId, @icsId)",
						new SqlParameters {
							["clientId"] = myDataRow["ClientID"],
							["caseId"] = myDataRow["CaseID"],
							["serviceId"] = programDetail.ProgramId,
							["locationId"] = _centerId,
							["serviceDate"] = programDetail.ServiceDate,
							["receivedHours"] = myDataRow["ReceivedHours"],
							["cityTownTownshipId"] = myDataRow["CityTownTownshpID"] ?? DBNull.Value,
							["fundDateId"] = programDetail.FundDateId,
							["agencyRecId"] = myDataRow["AgencyRecID"],
							["icsId"] = programDetail.IcsId
						}))
						cmd.ExecuteNonQuery();
				} else {
					try {
						if (ExistsInServiceDetailOfClient(programDetail.IcsId, (int)myDataRow["ClientID"], (int)myDataRow["CaseID"])) {
							using (var cmd = _infonet.Sql($"Update {TABLE_SQL_SERVICEDETAIL} Set ServiceID = @serviceId, ServiceDate = @serviceDate, ReceivedHours = @receivedHours, CityTownTownshpID = @cityTownTownshipId, FundDateID = @fundDateId, RevisionStamp = @revisionStamp, AgencyRecID = @agencyRecId WHERE ICS_ID = @icsId AND ClientID = @clientId AND CaseID = @caseId",
								new SqlParameters {
									["serviceId"] = programDetail.ProgramId,
									["serviceDate"] = programDetail.ServiceDate,
									["receivedHours"] = myDataRow["ReceivedHours"],
									["cityTownTownshipId"] = myDataRow["CityTownTownshpID"] ?? DBNull.Value,
									["fundDateId"] = programDetail.FundDateId,
									["revisionStamp"] = DateTime.Now,
									["agencyRecId"] = myDataRow["AgencyRecID"],
									["icsId"] = programDetail.IcsId,
									["clientId"] = myDataRow["ClientID"],
									["caseId"] = myDataRow["CaseID"]
								}))
								cmd.ExecuteNonQuery();
						} else {
							isAdd = true;
							using (var cmd = _infonet.Sql($"INSERT INTO {TABLE_SQL_SERVICEDETAIL} (ClientID, CaseID, ServiceID, LocationID, ServiceDate, ReceivedHours, CityTownTownshpID, FundDateID, AgencyRecID, ICS_ID) VALUES (@clientId, @caseId, @serviceId, @serviceDate, @receivedHours, @cityTownTownshipId, @fundDateId, @agencyRecId, @icsId)",
								new SqlParameters {
									["clientId"] = myDataRow["ClientID"],
									["caseId"] = myDataRow["CaseID"],
									["serviceId"] = programDetail.ProgramId,
									["locationId"] = _centerId,
									["serviceDate"] = programDetail.ServiceDate,
									["receivedHours"] = myDataRow["ReceivedHours"],
									["cityTownTownshipId"] = myDataRow["CityTownTownshpID"] ?? DBNull.Value,
									["fundDateId"] = programDetail.FundDateId,
									["agencyRecId"] = myDataRow["AgencyRecID"],
									["icsId"] = programDetail.IcsId
								}))
								cmd.ExecuteNonQuery();
						}
					} catch (SystemException ex) {
						LogException(ex);
						//KMS DO should return false, right?
					}
				}

			//KMS DO counts are updated even if we do nothing?  even if failed at previous KMS DO?
			if (isAdd)
				_addRecordCount++;
			else
				_updateRecordCount++;
			return true;
			//}
			//catch (SystemException ex)
			//{
			//    LogException(ex);
			//    //return false;
			//    throw;
			//}
		}

		private bool ExistsInProgramDetailStaffs(int icsId, int svId) {
			//try
			//{
			using (var cmd = _infonet.Sql($"Select count(*) from {TABLE_SQL_PROGRAMDETAIL_STAFFS} where ICS_ID = @p0 AND SVID = @p1", icsId, svId))
				return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
			//}
			//catch (SystemException ex)
			//{
			//    LogException(ex);
			//    //return false;
			//    throw;
			//}
		}

		private bool ExistsInServiceDetailOfClient(int icsId, int clientId, int caseId) {
			//try
			//{
			using (var cmd = _infonet.Sql($"Select count(*) from {TABLE_SQL_SERVICEDETAIL} where ICS_ID = @p0 AND ClientID = @p1 AND CaseID = @p2", icsId, clientId, caseId))
				return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
			//}
			//catch (SystemException ex)
			//{
			//    LogException(ex);
			//    //return false;
			//    throw;
			//}
		}

		private void UpdateTownTownshipCountyId() {
			//try
			//{
			if (!_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns.Contains("CityTownTownshpID"))
				_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT].Columns.Add(new DataColumn("CityTownTownshpID", typeof(long)) { DefaultValue = DBNull.Value });

			var servicesDataView = new DataView(_dataset.Tables[TABLE_SERVICEDETAIL_IMPORT]);
			var moveDatesDataView = new DataView(_dataset.Tables[TABLE_TOWNTOWNSHIPCOUNTY]) { Sort = "ClientID, MoveDate Desc" };
			for (int i = 0; i <= servicesDataView.Count - 1; i++) {
				var serviceDataRow = servicesDataView[i].Row;
				var serviceDate = ServiceDetailOfClient.AllShelterIds.Contains((int)serviceDataRow["ServiceID"]) ? (DateTime)serviceDataRow["ShelterBegDate"] : (DateTime)serviceDataRow["ServiceDate"];
				moveDatesDataView.RowFilter = "ClientID = " + serviceDataRow["ClientID"] + " AND ( MoveDate <= '" + serviceDate + "') ";
				if (moveDatesDataView.Count > 0) //KMS DO not while?
					serviceDataRow["CityTownTownshpID"] = moveDatesDataView[0].Row["LocID"];
			}
			//}
			//catch (Exception ex)
			//{
			//    LogException(ex);
			//    //throw new Exception("Unhandled Exception in Module: " + this, ex);
			//    throw;
			//}
		}
		#endregion private

		#region inner
		private class ProgramDetailSummary {
			internal int IcsId { get; set; }
			internal bool IsAdd { get; set; }
			internal int ProgramId { get; set; }
			internal DateTime ServiceDate { get; set; }
			internal int FundDateId { get; set; }
		}
		#endregion inner
	}
}