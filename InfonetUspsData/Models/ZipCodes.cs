using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Usps.Data.Models {
	public class ZipCodes {
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public ZipCodes() {
			ZipcodePlus4 = new HashSet<ZipcodePlus4>();
			Cities = new HashSet<Cities>();
			Counties = new HashSet<Counties>();
			States = new HashSet<States>();
			Townships = new HashSet<Townships>();
		}

		[Key]
		[StringLength(5)]
		public string Zipcode { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ZipcodePlus4> ZipcodePlus4 { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Cities> Cities { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Counties> Counties { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<States> States { get; set; }

		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Townships> Townships { get; set; }
	}
}