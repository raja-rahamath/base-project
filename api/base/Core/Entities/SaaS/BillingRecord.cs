using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Core.Entities.SaaS
{
    [Table("cor_billing_records")]
    public class BillingRecord : Entity
    {
        [Required]
        public Guid ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(10)]
        public string Currency { get; set; } = "USD";

        [Required]
        public DateTime BillingPeriodStart { get; set; }

        [Required]
        public DateTime BillingPeriodEnd { get; set; }

        [Required]
        public BillingStatus Status { get; set; } = BillingStatus.Due;

        [StringLength(255)]
        public string? Description { get; set; }

        public new DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? PaidAt { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }
    }

    public enum BillingStatus
    {
        Paid = 0,
        Due = 1,
        Overdue = 2,
        Cancelled = 3,
        Refunded = 4
    }
}
