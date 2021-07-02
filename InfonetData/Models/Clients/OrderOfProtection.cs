using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Infonet.Core.Collections;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;
using LinqKit;

namespace Infonet.Data.Models.Clients {
    [BindHint(Include = "StatusID,DateFiled,CountyID,DateIssued,DateVacated,OriginalExpirationDate,TypeOfOPID,ForumID,Comments,OrderOfProtectionActivitiesById,LocationID,CivilNoContactOrderTypeId,CivilNoContactOrderId,CivilNoContactOrderRequestId")]
    [DeleteIfNulled("ClientId,CaseId")]
    public class OrderOfProtection : IRevisable, IValidatableObject {
        public OrderOfProtection() {
            OrderOfProtectionActivities = new List<OpActivity>();
            OrderOfProtectionActivitiesById = new DerivedDictionary<OpActivity>(() => OrderOfProtectionActivities, true, e => e.OpActivityID?.ToString()) { Template = () => new OpActivity() };
        }

        public int? OP_ID { get; set; }

        public int? ClientId { get; set; }

        public int? CaseId { get; set; }

        [Lookup("OrderOfProtectionStatus")]
        [Display(Name = "Originally Sought Order")]
        public int? StatusID { get; set; }

        [BetweenNineteenSeventyToday]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date Filed")]
        public DateTime? DateFiled { get; set; }

        [Display(Name = "County")]
        public int? CountyID { get; set; }

        [BetweenNineteenSeventyToday]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Issue Date")]
        public DateTime? DateIssued { get; set; }

        [BetweenNineteenSeventyToday]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Vacate Date")]
        public DateTime? DateVacated { get; set; }

        [NotLessThanNineteenSeventy]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Original Expiration Date")]
        public DateTime? OriginalExpirationDate { get; set; }

        [Lookup("OrderOfProtectionType")]
        [Display(Name = "Order of Protection Type")]
        //KMS DO remember to change this in ClientCase validation(s) too
        public int? TypeOfOPID { get; set; }

        [Lookup("OrderOfProtectionForum")]
        [Display(Name = "Order of Protection Forum")]
        public int? ForumID { get; set; }

        [MaxLength(300, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        public string Comments { get; set; }

        public int? LocationID { get; set; }

        public DateTime? RevisionStamp { get; set; }

        [Lookup("OrderOfProtectionForum")]
        [Display(Name = "Civil No Contact Order Forum")]
        public int? CivilNoContactOrderId { get; set; }

        [Lookup("OrderOfProtectionType")]
        [Display(Name = "Civil No Contact Order Type")]
        public int? CivilNoContactOrderTypeId { get; set; }

        [Lookup("OrderOfProtectionStatus")]
        [Display(Name = "Civil No Contact Order Status")]
        public int? CivilNoContactOrderRequestId { get; set; }

        public virtual ClientCase ClientCase { get; set; }

        public virtual ICollection<OpActivity> OrderOfProtectionActivities { get; set; }

        [NotMapped]
        public virtual DerivedDictionary<OpActivity> OrderOfProtectionActivitiesById { get; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            var results = new List<ValidationResult>();

            if (DateIssued != null && DateFiled != null && DateIssued < DateFiled)
                results.Add(new ValidationResult("Issue Date must not be before Filed Date.", new[] { "DateIssued", "DateFiled" }));
            if (DateVacated != null && DateFiled != null && DateVacated < DateFiled)
                results.Add(new ValidationResult("Vacate Date must not be before Filed Date.", new[] { "DateVacated", "DateFiled" }));
            if (OriginalExpirationDate != null && DateIssued != null && OriginalExpirationDate < DateIssued)
                results.Add(new ValidationResult("Original Expiration Date must not be before Issue Date.", new[] { "DateIssued", "OriginalExpirationDate" }));

            foreach (var each in OrderOfProtectionActivitiesById)
                if (DateIssued != null && each.Value.OpActivityDate != null && each.Value.OpActivityDate < DateIssued)
                    results.Add(new ValidationResult("Activity Date must not be before Issue Date.", new[] { "OrderOfProtectionActivitiesById[" + each.Key + "].OpActivityDate", "DateIssued" }));

            return results;
        }

        #region predicates
        public static Expression<Func<OrderOfProtection, bool>> DateIssuedBetween(DateTime? minDateIssued, DateTime? maxDateIssued) {
            var predicate = PredicateBuilder.New<OrderOfProtection>(true);
            if (minDateIssued != null)
                predicate.And(s => s.DateIssued >= minDateIssued);
            if (maxDateIssued != null)
                predicate.And(s => s.DateIssued <= maxDateIssued);
            return predicate;
        }
        #endregion
    }
}