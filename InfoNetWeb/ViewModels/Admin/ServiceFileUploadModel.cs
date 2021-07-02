using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Infonet.Web.Mvc.Validation;

//KMS DO move and rename
namespace Infonet.Web.ViewModels.Admin {
	public class ServiceFileUploadModel {
		[Required(ErrorMessage = "An *.mdb file must be selected before importing services.")]
		//KMS DO change ChangeExtension calls to use .?
		[FileExtension(".mdb")]
		[Display(Name = "Selected file")]
		public HttpPostedFileBase UploadedFile { get; set; }

		public IEnumerable<AvailableFile> AvailableFiles { get; set; }

		public class AvailableFile {
			public string Name { get; set; }

			public long Size { get; set; }

			public DateTime LastModified { get; set; }  //KMS DO LastModifed? CreatedAt?
		}
	}	
}