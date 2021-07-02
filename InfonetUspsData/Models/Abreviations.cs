using System.ComponentModel.DataAnnotations;

namespace Infonet.Usps.Data.Models {
	public class Abreviations {
		public int ID { get; set; }

		[Required]
		[StringLength(10)]
		public string Abbreviation { get; set; }

		[Required]
		[StringLength(50)]
		public string UnAbbreviated { get; set; }
	}
}