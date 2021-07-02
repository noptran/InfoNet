using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Infonet.Core.Data;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Data.Models.Centers;

//KMS: DueDate would be a nice addition
namespace Infonet.Data.Models.Reporting {
	[BindHint(Include = "ApproverComment,Job,SystemMessage")]
	public class ReportJobApproval : IRevisable {
		private readonly DelimitedField<int> _centerIds = new DelimitedField<int>("|", "|", "|");

		public int? ReportJobId { get; set; }

		[Required]
		public string CenterIdsString {
			get { return _centerIds.Value; }
			set { _centerIds.Value = value; }
		}

		[NotMapped]
		public IEnumerable<int> CenterIds {
			get { return _centerIds.Items; }
			set { _centerIds.Items = value; }
		}

		[Required]
		public int? StatusId { get; set; }

		[Required]
		public DateTime? StatusDate { get; set; }

		public string ApproverUserName { get; set; }

		public string ApproverComment { get; set; }

		public int? SystemMessageId { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public byte[] RowVersion { get; set; }

		public virtual ReportJob Job { get; set; }

		public virtual SystemMessage SystemMessage { get; set; }

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public enum Status {
			ReviewOnly = 1,
			Pending = 2,
			Approved = 3,
			Rejected = 4
		}
	}
}