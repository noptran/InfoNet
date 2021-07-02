using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Usps.Data.Models {
	public class States {
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public States() {
			Cities = new HashSet<Cities>();
			Counties = new HashSet<Counties>();
			Townships = new HashSet<Townships>();
			ZipCodes = new HashSet<ZipCodes>();
		}

		public int ID { get; set; }

		[Required]
		[StringLength(32)]
		public string StateName { get; set; }

		[Required]
		[StringLength(2)]
		public string StateAbbreviation { get; set; }

		[Required]
		public int DisplayOrder { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Cities> Cities { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Counties> Counties { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Townships> Townships { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ZipCodes> ZipCodes { get; set; }
	}
}