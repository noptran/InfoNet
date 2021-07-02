using System;
using Infonet.Core.Entity;

namespace Infonet.Data.Models.Reporting {
	public class AdHocQuery : IRevisable {
		public int? Id { get; set; }
		public string Name { get; set; }
		public string Json { get; set; }
		public int? UserId { get; set; }
		public DateTime? RevisionStamp { get; set; }
	}
}