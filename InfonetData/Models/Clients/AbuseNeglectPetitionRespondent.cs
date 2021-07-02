using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;

namespace Infonet.Data.Models.Clients {
	[DeleteIfNulled("AbuseNeglectPetition_FK")]
	public class AbuseNeglectPetitionRespondent {
		public int? Id { get; set; }

		public int? AbuseNeglectPetition_FK { get; set; }

		[Required]
		public int RespondentId { get; set; }

		[Required]
		public int RespondentType { get; set; }

		public virtual AbuseNeglectPetition Petition { get; set; }
	}
}