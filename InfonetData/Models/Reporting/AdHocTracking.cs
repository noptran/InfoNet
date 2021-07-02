using System;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Reporting {
	public class AdHocTracking {
		public int? Id { get; set; }

		public string Json { get; set; }

		[Lookup("ReportOutput")]
		public int? OutputId { get; set; }

		public int? UserId { get; set; }

		public DateTime? RunDate { get; set; }

		public int? QueryId { get; set; }
	}
}