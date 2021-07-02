using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infonet.Usps.Data.Models {
	public class ZipcodePlus4 {
		[Key]
		[Column(Order = 0)]
		[StringLength(5)]
		public string Zipcode { get; set; }

		[Key]
		[Column(Order = 1)]
		[StringLength(4)]
		public string Suffix { get; set; }

		public virtual ZipCodes ZipCodes { get; set; }
	}
}