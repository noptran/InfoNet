using System;
using Infonet.Core.Entity;
using Infonet.Data.Looking;

//KMS DO rename to singular
namespace Infonet.Data.Models.Centers {
	public class CenterAdministrators : IRevisable {
		public int Id { get; set; }

		public int CenterId { get; set; }

		[Lookup("CenterAdministrator")]
		public int CenterAdminId { get; set; }

		public bool CenterAdminActive { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual Center Center { get; set; }
	}
}