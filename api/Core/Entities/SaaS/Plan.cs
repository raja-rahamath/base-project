using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Core.Entities.SaaS
{
    /// <summary>
    /// Represents a subscription plan
    /// </summary>
    [Table("cor_plans")]
    public class Plan : Entity
    {
        /// <summary>
        /// Plan name (e.g., Basic, Premium, Enterprise)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Plan description
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Monthly price of the plan
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyPrice { get; set; }
        
        /// <summary>
        /// Annual price of the plan
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AnnualPrice { get; set; }
        
        /// <summary>
        /// Maximum number of users allowed
        /// </summary>
        public int MaxUsers { get; set; }
        
        /// <summary>
        /// Maximum storage space in GB
        /// </summary>
        public int MaxStorageGB { get; set; }
        
        /// <summary>
        /// Features included in the plan (JSON string)
        /// </summary>
        [StringLength(1000)]
        public string Features { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether the plan is active or not
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Plan display order
        /// </summary>
        public int DisplayOrder { get; set; } = 0;
        
        /// <summary>
        /// Navigation property for client plans
        /// </summary>
        public virtual ICollection<ClientPlan> ClientPlans { get; set; } = new List<ClientPlan>();
    }
}