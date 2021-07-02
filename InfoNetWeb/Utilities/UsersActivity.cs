using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infonet.Web.Utilities {
	public static class UsersActivityList {
		private static readonly List<UserActivity> _Users = new List<UserActivity>();

        private static readonly object _UserLock = new object();

        public static IList<UserActivity> GetCurrentActiveUsers() {
            lock (_UserLock) {
                RemoveInactiveUsers();

                //only return authenticated users
                return _Users.Where(x => !string.IsNullOrWhiteSpace(x.UserName)).OrderBy(o => o.CenterName).ToList();
            }
		}

		public static void Update(UserActivity user) {
            lock (_UserLock) {
                if (_Users.Any()) {
                    var existing = _Users.SingleOrDefault(x => x.SessionId == user.SessionId);

                    if (existing != null) {
                        user.SessionStart = existing.SessionStart;
                        _Users.Remove(existing);
                    } else {
                        user.SessionStart = DateTime.Now;
                    }
                    RemoveInactiveUsers();
                }
                _Users.Add(user);
            }
		}

		private static void RemoveInactiveUsers() {
                foreach (var u in _Users.Where(x => (DateTime.Now - x.LastAccessed).Minutes > HttpContext.Current.Session.Timeout - 1).ToList())
				_Users.Remove(u);
		}
	}

	public class UserActivity {
		public int CenterId { get; set; }

		public string CenterName { get; set; }

		public DateTime LastAccessed { get; set; }

		public string SessionId { get; set; }

		public DateTime SessionStart { get; set; }

		public string UserName { get; set; }
	}
}