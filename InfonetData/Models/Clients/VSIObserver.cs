using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "ObserverID, ContactID, AgencyID")]
	// [DeleteIfNulled("ObserverID")]
	public class VSIObserver : IRevisable {
		public int? ID { get; set; }

		[Display(Name = "Agency")]
		public int? AgencyID { get; set; }

		[Display(Name = "Contact")]
		public int? ContactID { get; set; }

		[Required]
		[Display(Name = "Position")]
		[Lookup("ObserverPosition")]
		public int? ObserverID { get; set; }

		public int? VSI_ID { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual Agency Agency { get; set; }

		public virtual Contact Contact { get; set; }

		public virtual VictimSensitiveInterview VictimSensitiveInterview { get; set; }
	}
}