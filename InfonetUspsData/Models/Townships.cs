using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Usps.Data.Models {
	public class Townships {
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Townships() {
			Counties = new HashSet<Counties>();
			States = new HashSet<States>();
			Cities = new HashSet<Cities>();
			ZipCodes = new HashSet<ZipCodes>();
		}

		public int ID { get; set; }

		[Required]
		[StringLength(80)]
		public string TownshipName { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Counties> Counties { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<States> States { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Cities> Cities { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ZipCodes> ZipCodes { get; set; }
	}
}