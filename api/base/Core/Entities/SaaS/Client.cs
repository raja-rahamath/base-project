using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Core.Entities.SaaS
{
    /// <summary>
    /// Represents a SAAS client in the system
    /// </summary>
    [Table("cor_clients")]
    public class Client : Entity
    {
        /// <summary>
        /// Client's company name
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's VAT/tax number
        /// </summary>
        [StringLength(50)]
        public string VatNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's country
        /// </summary>
        [Required]
        [StringLength(2)]
        public string CountryCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's billing address (line 1)
        /// </summary>
        [StringLength(100)]
        public string BillingAddressLine1 { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's billing address (line 2)
        /// </summary>
        [StringLength(100)]
        public string BillingAddressLine2 { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's city
        /// </summary>
        [StringLength(50)]
        public string City { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's state/province
        /// </summary>
        [StringLength(50)]
        public string State { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's postal/ZIP code
        /// </summary>
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's primary contact email
        /// </summary>
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's phone number
        /// </summary>
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's website
        /// </summary>
        [StringLength(100)]
        public string Website { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's status (Active, Suspended, Terminated)
        /// </summary>
        [Required]
        public ClientStatus Status { get; set; } = ClientStatus.Active;
        
        /// <summary>
        /// Client's custom domain for accessing their application
        /// </summary>
        [StringLength(100)]
        public string DomainUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's database name
        /// </summary>
        [StringLength(50)]
        public string DatabaseName { get; set; } = string.Empty;
        
        /// <summary>
        /// Notes about the client
        /// </summary>
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;
        
        /// <summary>
        /// Navigation property for users
        /// </summary>
        public virtual ICollection<UserRef> Users { get; set; } = new List<UserRef>();
        
        /// <summary>
        /// Navigation property for client plans
        /// </summary>
        public virtual ICollection<ClientPlan> Plans { get; set; } = new List<ClientPlan>();
        
        /// <summary>
        /// Navigation property for client renewals
        /// </summary>
        public virtual ICollection<ClientRenewal> Renewals { get; set; } = new List<ClientRenewal>();
        
        /// <summary>
        /// Navigation property for billing records
        /// </summary>
        public virtual ICollection<BillingRecord> BillingRecords { get; set; } = new List<BillingRecord>();
    }
    
    /// <summary>
    /// Client status enum
    /// </summary>
    public enum ClientStatus
    {
        /// <summary>
        /// Client is active
        /// </summary>
        Active = 0,
        
        /// <summary>
        /// Client is suspended
        /// </summary>
        Suspended = 1,
        
        /// <summary>
        /// Client is terminated
        /// </summary>
        Terminated = 2
    }
}