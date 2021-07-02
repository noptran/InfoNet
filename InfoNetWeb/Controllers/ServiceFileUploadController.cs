using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web.Mvc;
using Infonet.Data.Importing;
using Infonet.Web.Mvc;
using Infonet.Web.ViewModels.Admin;

/*
 * 
 *  Login to test with : comm000151 or wing000247
 *  Test files are at : \\msfw-adspi-ps\MSF&W Clients\Clients - Consulting\Criminal Justice Information Authority\2016 InfoNet\Upload Testing
 *  
 */
namespace Infonet.Web.Controllers {
	[Authorize(Roles = "DATAIMPORTER")]
	public class ServiceFileUploadController : InfonetControllerBase {
		private DirectoryInfo _ImportDirectory {
			get {
				string temp = ConfigurationManager.AppSettings["ImportDirectory"]; // Path.GetFullPath();
				if (temp == null)
					temp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\Uploads");
				var di = new DirectoryInfo(temp);
				if (di.Exists == false)
					di.Create();
				return di;
			}
		}

		private string CenterImportDirectory {
			get {
				string path = Path.Combine(_ImportDirectory.FullName, Session.Center().Id.ToString());
				Directory.CreateDirectory(path);
				return path;
			}
		}

		private IEnumerable<ServiceFileUploadModel.AvailableFile> AvailableFiles {
			get {
				var result = new List<ServiceFileUploadModel.AvailableFile>();
				if (Directory.Exists(CenterImportDirectory))
					foreach (var each in new DirectoryInfo(CenterImportDirectory).GetFiles())
						result.Add(new ServiceFileUploadModel.AvailableFile {
							Name = each.Name,
							Size = each.Length,
							LastModified = each.LastWriteTime
						});
				return result;
			}
		}

		private static bool HasWritePermissionOnDir(string path) {
			bool writeAllow = false;
			bool writeDeny = false;
			var accessRules = Directory.GetAccessControl(path)?.GetAccessRules(true, true, typeof(SecurityIdentifier));
			if (accessRules == null)
				return false;

			foreach (FileSystemAccessRule rule in accessRules) {
				if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
					continue;

				if (rule.AccessControlType == AccessControlType.Allow)
					writeAllow = true;
				else if (rule.AccessControlType == AccessControlType.Deny)
					writeDeny = true;
			}

			return writeAllow && !writeDeny;
		}

		[HttpGet]
		public ActionResult Index() {
			if (HasWritePermissionOnDir(CenterImportDirectory) == false)
				AddErrorMessage("System does not have write permission to the server directory: " + CenterImportDirectory);

			if (Directory.Exists(CenterImportDirectory) == false)
				AddErrorMessage("Server directory does not exist : " + CenterImportDirectory);

			return View(new ServiceFileUploadModel { AvailableFiles = AvailableFiles });
		}

		//KMS DO rename
		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
		public ActionResult Upload(ServiceFileUploadModel model) {
			string mdbPath = Path.Combine(CenterImportDirectory, Path.GetFileName(model.UploadedFile.FileName));
            
            if (Path.GetFileName(model.UploadedFile.FileName).ToLower() != "servicedetail_" + Session.Center().Id + ".mdb")
				ModelState.AddModelError("UploadedFile", "ServiceDetail Import file must be called \"ServiceDetail_" + Session.Center().Id + ".mdb\" to get imported into the system.");

			if (System.IO.File.Exists(mdbPath))
				ModelState.AddModelError("UploadedFile", "Delete the old " + model.UploadedFile.FileName + " and then upload the new one.");

			if (!ModelState.IsValid) {
				model.AvailableFiles = AvailableFiles;
				return View("Index", model);
			}

			model.UploadedFile.SaveAs(mdbPath);

			//KMS DO what if it already exists?  this is more complicated now that we have csv files to worry about too

			/*
				<add key="InfonetServerDatabase_ConnectionString" value="Connection Lifetime=0;Connect Timeout=600" />
				<add key="InfonetServerDatabase_CommandTimeout" value="60" />
				<add key="JetOLEDBVersion" value="Microsoft.Jet.OleDb.4.0" />
				<add key="ServiceDetailImport.FileSystemWatcher.DeleteMDBFileAfterImport" value="true" />
				<add key="ValidateOnly" value="false" />	
				<add key="AssignTownTownShipCountyID" value="true"/>
				<add key="AssignFundingDateID" value="true"/>
				<add key="AllowServiceBeforeFirstContact" value="true"/>
				<add key="AllowZeroServiceHours" value="false"/>
			*/

			int sqlCommandTimeout = 60; //KMS DO what should this be?  //KMS DO move to app config? -- how about just part of the connection string?

			try {
				/* try { */
				using (var dataSet1 = new ServicesImport(Session.Center().ProviderId, Session.Center().Id, mdbPath, ConfigurationManager.ConnectionStrings["InfonetServerContext"].ConnectionString, sqlCommandTimeout)) {
					//.AllowServiceBeforeFirstContact = CType(MyAppSettings.GetValue("AllowServiceBeforeFirstContact", GetType(System.Boolean)), Boolean)
					//.AllowZeroServiceHours = CType(MyAppSettings.GetValue("AllowZeroServiceHours", GetType(System.Boolean)), Boolean)
					//.AssignTownTownShipCountyID = CType(MyAppSettings.GetValue("AssignTownTownShipCountyID", GetType(System.Boolean)), Boolean)
					//.AssignFundingDateID = CType(MyAppSettings.GetValue("AssignFundingDateID", GetType(System.Boolean)), Boolean)
					//.ValidateOnly = CType(MyAppSettings.GetValue("ValidateOnly", GetType(System.Boolean)), Boolean)

					dataSet1.LoadAndValidate();
					dataSet1.SaveToSqlServer();
					dataSet1.DumpExceptions();
					dataSet1.WriteStatisticsToLog();
				}
				/*
							} catch (Exception ex1) {
								m_EventLog.WriteEntry("Exception: " & ex1.ToString() & vbCrLf & "Source: " & ex1.Source & vbCrLf & "Message:" & vbCrLf & ex1.Message & vbCrLf & "StackTrace: " & vbCrLf & ex1.StackTrace, EventLogEntryType.Error)
							} finally {
								//Remove dump File if indicated in config file.
								If CType(MyAppSettings.GetValue("ServiceDetailImport.FileSystemWatcher.DeleteMDBFileAfterImport", GetType(System.Boolean)), System.Boolean) Then
									MyFileInfo.Delete()
								End If
							}
					*/
			} catch (Exception e) {
				AddErrorMessage($"<strong><i>{Path.GetFileName(mdbPath)}</i></strong>: {e.Message}.");
			}
			return RedirectToAction("Index");
		}

		//KMS DO review this closely
		public ActionResult Delete(string fileName) {
			string path = Path.Combine(CenterImportDirectory, fileName);
			if (System.IO.File.Exists(path))
				System.IO.File.Delete(path);
			return RedirectToAction("Index");
		}

		//KMS DO reads entire file into memory
		//KMS DO might be able to download files that don't belong to requestor
		public FileResult Download(string fileName) {
			string path = Path.Combine(CenterImportDirectory, fileName);
			if (!System.IO.File.Exists(path))
				throw new FileNotFoundException(path + "Can not be found!");
			var fileBytes = System.IO.File.ReadAllBytes(path);
			return File(fileBytes, MediaTypeNames.Application.Octet, fileName);
		}
	}
}