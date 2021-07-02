using System;
using System.Linq;
using Infonet.Core.Logging;
using Infonet.Core.Threading;
using Infonet.Data;
using Infonet.Data.Models.Reporting;

namespace Infonet.Reporting.Service {
	public class ReportJobSource : IFetchable<ReportJob.Ticket> {
		private readonly string _name;
		private readonly ReportJob.Status _workStatus;
		private readonly Func<InfonetServerContext, IQueryable<ReportJob>> _workQuery;

		public ReportJobSource(string name, ReportJob.Status workStatus, Func<InfonetServerContext, IQueryable<ReportJob>> workQuery) {
			_name = name;
			_workStatus = workStatus;
			_workQuery = workQuery;
		}

		public int Fetch(ReportJob.Ticket[] buffer) {
			return Fetch(buffer, 0, buffer?.Length ?? 0);
		}

		public int Fetch(ReportJob.Ticket[] buffer, int offset, int count) {
			using (var db = new InfonetServerContext()) {
				var jobs = _workQuery(db).Take(count).ToArray();
				foreach (var each in jobs)
					Log.Debug(each.EnterStatus(_workStatus));
				if (jobs.Length > 0)
					db.SaveChanges();

				int result = 0;
				foreach (var each in jobs)
					buffer[offset + result++] = each.ToTicket();
				return result;
			}
		}

		public override string ToString() {
			return $"{GetType().Name}[{_name}]";
		}
	}
}