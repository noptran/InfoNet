using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.ViewModels.Case {
	public class OtherCase {
		public int CaseId { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "First Contact Date")]
		public DateTime FirstContactDate { get; set; }

		public string CaseDisplay {
			get { return $"{CaseId} - {FirstContactDate:MM/dd/yyyy}"; }
		}

		public bool IsClosed { get; set; }
	}
}