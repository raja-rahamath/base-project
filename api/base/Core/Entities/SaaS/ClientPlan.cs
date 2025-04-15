using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Core.Entities.SaaS
{
    /// <summary>
    /// Represents a client's subscription plan
    /// </summary>
    [Table("cor_client_plans")]
    public class ClientPlan : Entity
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
        /// Reference to the plan
        /// </summary>
        [Required]
        public Guid PlanId { get; set; }
        
        /// <summary>
        /// Navigation property for plan
        /// </summary>
        [ForeignKey("PlanId")]
        public virtual Plan Plan { get; set; } = null!;
        
        /// <summary>
        /// Start date of the subscription
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// End date of the subscription
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Whether the subscription is annual or monthly
        /// </summary>
        [Required]
        public BillingCycle BillingCycle { get; set; } = BillingCycle.Monthly;
        
        /// <summary>
        /// Price at the time of purchase
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        /// <summary>
        /// Whether the subscription is active or not
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Whether the subscription auto-renews
        /// </summary>
        [Required]
        public bool AutoRenew { get; set; } = true;
        
        /// <summary>
        /// Notes about the subscription
        /// </summary>
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        /// <summary>
        /// Navigation property for client renewals
        /// </summary>
        public virtual ICollection<ClientRenewal> Renewals { get; set; } = new List<ClientRenewal>();
    }
    
    /// <summary>
    /// Billing cycle enum
    /// </summary>
    public enum BillingCycle
    {
        /// <summary>
        /// Monthly billing
        /// </summary>
        Monthly = 0,
        
        /// <summary>
        /// Annual billing
        /// </summary>
        Annual = 1
    }
}