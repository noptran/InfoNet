using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Clients {
	public class ClientMDT : IRevisable {
		public int? MDT_ID { get; set; }

		public int ClientID { get; set; }

		public int CaseID { get; set; }

		[Required]
		[Display(Name = "Contact")]
		public int? ContactID { get; set; }

		[Required]
		[Display(Name = "Agency")]
		public int? AgencyID { get; set; }

		[Required]
		[Lookup("TeamMemberPosition")]
		[Display(Name = "Position")]
		public int? PositionID { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual Agency Agency { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		public virtual Contact Contact { get; set; }
	}
}