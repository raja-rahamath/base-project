using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Core.Entities.SaaS
{
    /// <summary>
    /// Represents a client's subscription renewal
    /// </summary>
    [Table("cor_client_renewals")]
    public class ClientRenewal : Entity
    {
        /// <summary>
        /// Reference to the client
        /// </summary>
        [Required]
        public Guid ClientId { get; set; }
        
        /// <summary>
        /// Navigation property for client
        /// </summary>
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; } = null!;
        
        /// <summary>
        /// Reference to the client plan
        /// </summary>
        [Required]
        public Guid ClientPlanId { get; set; }
        
        /// <summary>
        /// Navigation property for client plan
        /// </summary>
        [ForeignKey("ClientPlanId")]
        public virtual ClientPlan ClientPlan { get; set; } = null!;
        
        /// <summary>
        /// Previous plan end date
        /// </summary>
        [Required]
        public DateTime PreviousEndDate { get; set; }
        
        /// <summary>
        /// New plan start date
        /// </summary>
        [Required]
        public DateTime NewStartDate { get; set; }
        
        /// <summary>
        /// New plan end date
        /// </summary>
        [Required]
        public DateTime NewEndDate { get; set; }
        
        /// <summary>
        /// Renewal amount
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Payment date
        /// </summary>
        public DateTime? PaymentDate { get; set; }
        
        /// <summary>
        /// Payment method used
        /// </summary>
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;
        
        /// <summary>
        /// Transaction reference number
        /// </summary>
        [StringLength(100)]
        public string TransactionReference { get; set; } = string.Empty;
        
        /// <summary>
        /// Renewal status
        /// </summary>
        [Required]
        public RenewalStatus Status { get; set; } = RenewalStatus.Pending;
        
        /// <summary>
        /// Notes about the renewal
        /// </summary>
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Renewal status enum
    /// </summary>
    public enum RenewalStatus
    {
        /// <summary>
        /// Renewal is pending
        /// </summary>
        Pending = 0,
        
        /// <summary>
        /// Renewal payment is completed
        /// </summary>
        Completed = 1,
        
        /// <summary>
        /// Renewal payment failed
        /// </summary>
        Failed = 2,
        
        /// <summary>
        /// Renewal was cancelled
        /// </summary>
        Cancelled = 3
    }
}