using System.Collections.Generic;
using System.Linq;
using System.Web;
using Infonet.Data;
using Infonet.Web.Mvc.Authorization;

namespace Infonet.Web.Mvc {
	public static class HttpSessionStateBaseExtensions {
		private const string CENTER = "_Center";
		private const string LAST_FIELD_ID = "_LastFieldId";
		private const string RECENT_CASES = "_LastSelectedClientIds";

		public static SessionCenter Center(this HttpSessionStateBase session) {
			var center = (SessionCenter)session[CENTER];
			if (center == null)
				using (var db = new InfonetServerContext()) {
					int centerId = HttpContext.Current.User.Identity.GetCenterId();
					var parent = new SessionCenter(db.T_Center.Single(c => c.Satellites.Any(s => s.CenterID == centerId)), null);
					session[CENTER] = center = parent.FindRelated(centerId);
				}
			return center;
		}

		public static long NextFieldId(this HttpSessionStateBase session) {
			return (long)(session[LAST_FIELD_ID] = ((long?)session[LAST_FIELD_ID] ?? 0) + 1);
		}

		public static List<RecentClient> RecentClients(this HttpSessionStateBase session) {
			var recentCases = (List<RecentClient>)session[RECENT_CASES];
			if (recentCases == null)
				session[RECENT_CASES] = recentCases = new List<RecentClient>();
			return recentCases;
		}

		public static void IncludeRecentClient(this HttpSessionStateBase session, int clientId, int caseId, string clientCode, string action) {
			var recentCases = session.RecentClients();
			recentCases.RemoveAll(cc => cc.ClientId == clientId);
			if (recentCases.Count > 4)
				recentCases.RemoveRange(4, recentCases.Count - 4);
			recentCases.Insert(0, new RecentClient(clientId, caseId, clientCode, action));
		}
	}
}