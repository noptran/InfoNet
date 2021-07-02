using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Core.Logging;
using Infonet.Core.Threading;
using Infonet.Data;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models.Reporting;
using Infonet.Reporting.Core;
using Infonet.Reporting.StandardReports;
using Newtonsoft.Json;
using ThreadState = System.Threading.ThreadState;

namespace Infonet.Reporting.Service {
	public class ReportService : ServiceBase {
		#region constants
		// ReSharper disable once MemberCanBePrivate.Global
		public const int DEFAULT_RETRY_AFTER_SECONDS = 300;

		private const string TMP = ".tmp";
		private const string HTML = ".html";
		private const string CSV = ".csv";
		#endregion

		#region static configuration
		private static readonly int? _QueryTimeoutMilliseconds = ConvertNull.ToInt32(ConfigurationManager.AppSettings["QueryTimeoutSeconds"]) * 1000;
		private static readonly int _RetryAfterSeconds = ConvertNull.ToInt32(ConfigurationManager.AppSettings["RetryAfterSeconds"]) ?? DEFAULT_RETRY_AFTER_SECONDS;
		private static readonly string _OutputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];
		private static readonly string _HelpDeskEmail = ConfigurationManager.AppSettings["HelpDeskEmail"];
		private static readonly string _HelpDeskPhone = ConfigurationManager.AppSettings["HelpDeskPhone"];
		private static readonly bool _EnableEmail = ConvertNull.ToBoolean(ConfigurationManager.AppSettings["EnableEmail"]) ?? false;
		#endregion

		#region job sources
		private readonly ClosingFetchable<ReportJob.Ticket> _jobsToRun =
			new ClosingFetchable<ReportJob.Ticket>(
				new LoggingFetchable<ReportJob.Ticket>(
					new ReportJobSource("Runner", ReportJob.Status.Fetching, db => {
						var now = DateTime.Now;
						return db.RPT_ReportJobs
							.Where(j => ReportJob.WaitingStatuses.Contains((ReportJob.Status)j.StatusId) && j.ScheduledForDate <= now && (j.ExpirationDate == null || j.ExpirationDate > now))
							.OrderByDescending(j => j.Priority).ThenBy(j => j.ScheduledForDate).ThenBy(j => j.SubmittedDate).ThenBy(j => j.Id);
					})));

		private readonly ClosingFetchable<ReportJob.Ticket> _jobsToDelete =
			new ClosingFetchable<ReportJob.Ticket>(
				new LoggingFetchable<ReportJob.Ticket>(
					new ReportJobSource("Deleter", ReportJob.Status.Deleting, db => {
						var now = DateTime.Now;
						return db.RPT_ReportJobs
							.Where(j => !ReportJob.ActiveStatuses.Contains((ReportJob.Status)j.StatusId) && j.StatusId != (int)ReportJob.Status.Hold && j.ExpirationDate <= now)
							.OrderBy(j => j.Id);
					})));
		#endregion

		private readonly string _id = Environment.MachineName + "/" + Process.GetCurrentProcess().Id + "/" + DateTime.UtcNow.Ticks;
		private readonly Thread[] _threadsForRunning = new Thread[Convert.ToInt32(ConfigurationManager.AppSettings["ThreadCount"] ?? "8")];
		private readonly Thread[] _threadsForDeleting = new Thread[1];
		private readonly PollingFetchable<ReportJob.Ticket> _pollToRun;
		private readonly PollingFetchable<ReportJob.Ticket> _pollToDelete;

		#region constructing/disposing
		public ReportService() {
			ServiceName = "InfonetReportingService";
			_pollToRun = new PollingFetchable<ReportJob.Ticket>(_jobsToRun, ConvertNull.ToInt32(ConfigurationManager.AppSettings["PollingIntervalSeconds"]) * 1000);
			_pollToDelete = new PollingFetchable<ReportJob.Ticket>(_jobsToDelete, 60 * 60 * 1000);
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				_pollToRun.Dispose();
				_pollToDelete.Dispose();
			}
			base.Dispose(disposing);
		}
		#endregion

		protected override void OnStart(string[] args) {
			Log.Debug($"{this} started");

			ReportContainer.InitializeRazor();

			if (!Directory.Exists(_OutputDirectory)) {
				Log.Debug($"{this} creating directory: {_OutputDirectory}");
				Directory.CreateDirectory(_OutputDirectory);
			}

			RepairJobs();

			for (int i = 0; i < _threadsForRunning.Length; i++) {
				var thread = new Thread(RunJobs);
				thread.Name = _id + "/" + thread.ManagedThreadId;
				(_threadsForRunning[i] = thread).Start();
			}

			for (int i = 0; i < _threadsForDeleting.Length; i++) {
				var thread = new Thread(DeleteJobs);
				thread.Name = _id + "/" + thread.ManagedThreadId;
				(_threadsForDeleting[i] = thread).Start();
			}
		}

		protected override void OnStop() {
			Log.Debug($"{this} stopping...");
			_jobsToRun.Close();
			_jobsToDelete.Close();
			_pollToRun.WaitNoMore();
			_pollToDelete.WaitNoMore();
			JoinRemaining();
			ReportContainer.DisposeRazor();
			Log.Debug($"{this} stopped");
		}

		public void Flush() {
			Log.Debug($"{this} flushing...");
			_jobsToRun.CloseOnEmpty();
			_jobsToDelete.CloseOnEmpty();
			_pollToRun.RetryAfterMilliseconds = _pollToDelete.RetryAfterMilliseconds = 0;
			OnStart(null);
			JoinRemaining();
			ReportContainer.DisposeRazor();
			Log.Debug($"{this} stopped");
		}

		public void Debug() {
			Log.Debug($"{this} debugging...");
			OnStart(null);
			//NOTE RazorEngine temp files are cleaned up when debugger is stopped
		}

		public override string ToString() {
			return $"{GetType().Name}[{_id}]";
		}

		#region private
		private void RepairJobs() {
			Log.Debug($"{this} scanning for {nameof(ReportJob)}s to repair...");
			using (var db = new InfonetServerContext()) {
				var query = db.RPT_ReportJobs.Where(rj => ReportJob.ActiveStatuses.Contains((ReportJob.Status)rj.StatusId) &&
														(rj.ActiveThread == null || rj.ActiveThread.StartsWith(Environment.MachineName + "/")));
				foreach (var each in query) {
					string message = $"{this} repairing {nameof(ReportJob)}[{each.Id}]: found stuck in {(ReportJob.Status)each.StatusId} status";
					each.Log(message);
					Log.Debug(message);
					if (each.StatusId == (int)ReportJob.Status.Deleting) {
						Log.Debug(each.EnterStatus(ReportJob.Status.DeleteFailed));
					} else if (each.RemainingTries > 0) {
						Log.Debug(each.EnterStatus(ReportJob.Status.Error));
						each.ScheduledForDate += new TimeSpan(0, 0, _RetryAfterSeconds);
					} else {
						Log.Debug(each.EnterStatus(ReportJob.Status.Failed));
					}
				}
				db.SaveChanges();
			}
		}

		private void RunJobs() {
			string logId = $"{GetType().Name}.Thread[{Thread.CurrentThread.Name}]";
			try {
				Log.Debug($"{logId} started");
				var tickets = new ReportJob.Ticket[1];
				while (_pollToRun.Fetch(tickets) >= 0)
					try {
						using (var db = new InfonetServerContext()) {
							var ticket = tickets[0];
							var job = db.RPT_ReportJobs.SingleOrDefault(rj => rj.Id == ticket.Id && rj.RowVersion == ticket.RowVersion);
							if (job == null) {
								Log.Debug($"{logId} skipping {ticket} (externally modified after fetching but before loading)");
								continue;
							}

							job.RemainingTries--;
							Log.Debug(job.EnterStatus(ReportJob.Status.Running));
							db.SaveChanges();

							try {
								var specification = JsonConvert.DeserializeObject<StandardReportSpecification>(job.SpecificationJson, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });

								string jobDirectory = Path.Combine(_OutputDirectory, job.Id.ToString());
								string htmlPath = Path.Combine(jobDirectory, job.Id + HTML);
								string csvPath = Path.Combine(jobDirectory, job.Id + CSV);
								if (File.Exists(htmlPath))
									File.Delete(htmlPath);
								if (File.Exists(csvPath))
									File.Delete(csvPath);

								using (var report = StandardReportFactory.RunStandardReport(specification)) {
									report.CommandTimeout = _QueryTimeoutMilliseconds;
									Directory.CreateDirectory(jobDirectory);
									using (var html = new StreamWriter(new FileStream(htmlPath + TMP, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8))
									using (var csv = new StreamWriter(new FileStream(csvPath + TMP, FileMode.Create, FileAccess.Write, FileShare.Read), Encoding.UTF8))
										report.Write(html, csv);
								}
								File.Move(htmlPath + TMP, htmlPath);
								File.Move(csvPath + TMP, csvPath);

								bool reviewOnly = job.Approval?.StatusId == ReportJobApproval.Status.ReviewOnly.ToInt32();
								if (reviewOnly || job.Approval?.StatusId == ReportJobApproval.Status.Pending.ToInt32()) {
									//post a SystemMessage
									var systemMessage = job.Approval.SystemMessage ?? (job.Approval.SystemMessage = new SystemMessage { LocationIdsString = job.Approval.CenterIdsString });
									systemMessage.Title = reviewOnly ? "Report Ready" : "Report Approval Needed";
									systemMessage.Message = $"<i>{job.SubmitterCenter.CenterName}</i> has generated a report for your {(reviewOnly ? "review" : "approval")}: <strong>{specification.Title}</strong>.";
									systemMessage.IsHot = !reviewOnly;
									systemMessage.LinkUrl = $"/Report/CompletedReport?RptId={job.Id}";
									systemMessage.LinkText = reviewOnly ? "Review" : "Approve/Reject";
									systemMessage.PostedDate = DateTime.Now;
									systemMessage.ExpirationDate = job.ExpirationDate;

									//send an email
									if (!reviewOnly) {
										var emailAddresses = GetDirectorEmailAddresses(db, job.Approval.CenterIds);
										if (emailAddresses.Length == 0) {
											job.Log("Unable to send email: DirectorEmail(s) missing");
											Log.Debug($"{job.DisplayName} unable to send email: DirectorEmail(s) missing");
										} else {
											using (var smtp = new SmtpClient())
											using (var email = new MailMessage { IsBodyHtml = true }) {
												foreach (string each in emailAddresses)
													email.To.Add(each);
												email.Subject = $"InfoNet Report Approval Needed: {specification.Title}";
												email.Body = ComposeEmailBody(job, specification);
												job.Log($"Sending email to {emailAddresses.ToConjoinedString("and")}");
												Log.Debug(job.DisplayName, email);
												if (_EnableEmail)
													try {
														smtp.Send(email);
													} catch (Exception e) {
														job.Log(e.ToString());
														Log.Debug(e.ToString());
													}
											}
										}
									}
								}

								Log.Debug(job.EnterStatus(ReportJob.Status.Succeeded));
							} catch (Exception e) {
								job.Log(e.ToString());
								Log.Debug(e.ToString());
								if (job.RemainingTries > 0) {
									Log.Debug(job.EnterStatus(ReportJob.Status.Error));
									job.ScheduledForDate = job.StatusDate + new TimeSpan(0, 0, _RetryAfterSeconds);
									job.Log($"{nameof(job.ScheduledForDate)} advanced to {job.ScheduledForDate} for retry");
								} else {
									Log.Debug(job.EnterStatus(ReportJob.Status.Failed));
								}
							}
							db.SaveChanges();
						}
					} catch (Exception e) {
						Log.Debug(e.ToString());
						Log.Debug($"{logId} failed unexpectedly while processing {tickets[0]}");
					}
				Log.Debug($"{logId} stopped");
			} catch (Exception e) {
				Log.Debug(e.ToString());
				Log.Debug($"{logId} failed unexpectedly (i.e. something really bad happened)");
			}
		}

		private void DeleteJobs() {
			string logId = $"{GetType().Name}.Thread[{Thread.CurrentThread.Name}]";
			try {
				Log.Debug($"{logId} started");
				var tickets = new ReportJob.Ticket[1];
				while (_pollToDelete.Fetch(tickets) >= 0)
					try {
						using (var db = new InfonetServerContext()) {
							var ticket = tickets[0];
							var job = db.RPT_ReportJobs.SingleOrDefault(rj => rj.Id == ticket.Id && rj.RowVersion == ticket.RowVersion);
							if (job == null) {
								Log.Debug($"{logId} skipping {ticket} (externally modified while deleting)");
								continue;
							}

							Log.Debug($"{logId} attempting to remove directory/files for {ticket}");
							try {
								string jobDirectory = Path.Combine(_OutputDirectory, job.Id.ToString());
								string htmlPath = Path.Combine(jobDirectory, job.Id + HTML);
								string csvPath = Path.Combine(jobDirectory, job.Id + CSV);
								if (File.Exists(htmlPath + TMP))
									File.Delete(htmlPath + TMP);
								if (File.Exists(csvPath + TMP))
									File.Delete(csvPath + TMP);
								if (File.Exists(htmlPath))
									File.Delete(htmlPath);
								if (File.Exists(csvPath))
									File.Delete(csvPath);
								if (Directory.Exists(jobDirectory))
									Directory.Delete(jobDirectory);

								string message = $"{logId} successfully removed directory/files for {ticket}";
								job.Log(message);
								Log.Debug(message);
								if (job.Approval != null) {
									var systemMessage = job.Approval.SystemMessage;
									db.RPT_ReportJobApprovals.Remove(job.Approval);
									if (systemMessage != null)
										db.T_SystemMessages.Remove(systemMessage);
								}
								db.RPT_ReportJobs.Remove(job);
								db.SaveChanges();
								Log.Debug($"{logId} successfully deleted {ticket}");
							} catch (Exception e) {
								db.Entry(job).State = EntityState.Modified;
								job.Log(e.ToString());
								Log.Debug(e.ToString());
								Log.Debug(job.EnterStatus(ReportJob.Status.DeleteFailed));
								job.ExpirationDate = job.StatusDate + new TimeSpan(1, 0, 0, 0);
								job.Log($"{nameof(job.ExpirationDate)} advanced to {job.ExpirationDate} for retry");
								db.SaveChanges();
							}
						}
					} catch (Exception e) {
						Log.Debug(e.ToString());
						Log.Debug($"{logId} failed unexpectedly while processing {tickets[0]}");
					}
				Log.Debug($"{logId} stopped");
			} catch (Exception e) {
				Log.Debug(e.ToString());
				Log.Debug($"{logId} failed unexpectedly (i.e. something really bad happened)");
			}
		}

		private void JoinRemaining() {
			foreach (var each in _threadsForRunning.Concat(_threadsForDeleting))
				if (each != null)
					if (each.ThreadState == ThreadState.Running || each.ThreadState == ThreadState.WaitSleepJoin)
						each.Join();
		}
		#endregion

		#region private static
		private static string[] GetDirectorEmailAddresses(InfonetServerContext db, IEnumerable<int> approvalCenterIds) {
			var emailAddressQuery = db.T_Center.Where(c => approvalCenterIds.Contains(c.CenterID) && c.DirectorEmail != null && c.DirectorEmail.Trim() != "").Select(c => c.DirectorEmail);
			var emailAddresses = new HashSet<string>(emailAddressQuery, StringComparer.CurrentCultureIgnoreCase).ToArray();
			Array.Sort(emailAddresses);
			return emailAddresses;
		}

		private static string ComposeEmailBody(ReportJob job, StandardReportSpecification specification) {
			using (var sb = new StringWriter()) {
				sb.Write($"This email is to notify you that the following report generated by <i>{job.SubmitterCenter.CenterName}</i> on {job.SubmittedDate:M/d/yyyy} is ready for approval: <strong>{specification.Title}</strong>.<br/><br/>Please approve/reject this report by the due date specified by <i>{job.SubmitterCenter.CenterName}</i>.");
				if (_HelpDeskEmail != null || _HelpDeskPhone != null) {
					sb.Write("<br/><br/>If you have any questions or need technical assistance with InfoNet, please contact the help desk at ");
					if (_HelpDeskEmail != null) {
						sb.Write($"<a href=\"mailto:{_HelpDeskEmail}\">{_HelpDeskEmail}</a>");
						if (_HelpDeskPhone != null)
							sb.Write(" or ");
					}
					if (_HelpDeskPhone != null)
						sb.Write($"<a href=\"tel:+1-{_HelpDeskPhone}\">{_HelpDeskPhone}</a>");
					sb.Write(".");
				}
				return sb.ToString();
			}
		}
		#endregion
	}
}