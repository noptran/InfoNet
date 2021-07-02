namespace Infonet.Web.Mvc {
	public class RecentClient {
		internal RecentClient(int clientId, int caseId, string clientCode, string action) {
			ClientId = clientId;
			CaseId = caseId;
			ClientCode = clientCode;
			Action = action;
		}

		public int ClientId { get; }
		public int CaseId { get; }
		public string ClientCode { get; }
		public string Action { get; }
	}
}