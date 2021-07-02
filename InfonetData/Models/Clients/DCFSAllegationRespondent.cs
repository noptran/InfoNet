using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;

namespace Infonet.Data.Models.Clients {
	[DeleteIfNulled("DCFSAllegations_FK")]
	public class DCFSAllegationRespondent {
		public int? Id { get; set; }

		public int? DCFSAllegations_FK { get; set; }

		[Required]
		public int RespondentId { get; set; }

		[Required]
		public int RespondentType { get; set; }

		public virtual DCFSAllegation Allegation { get; set; }
	}
}