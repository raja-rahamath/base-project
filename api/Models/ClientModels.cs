using api.Core.Entities.SaaS;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Models
{
    /// <summary>
    /// Request model for client registration
    /// </summary>
    public class ClientRegistrationRequest
    {
        /// <summary>
        /// Client's company name
        /// </summary>
        [Required]
        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's VAT/tax number
        /// </summary>
        [JsonPropertyName("vatNumber")]
        public string? VatNumber { get; set; }
        
        /// <summary>
        /// Client's country (ISO code)
        /// </summary>
        [Required]
        [StringLength(2)]
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's billing address (line 1)
        /// </summary>
        [JsonPropertyName("billingAddressLine1")]
        public string? BillingAddressLine1 { get; set; }
        
        /// <summary>
        /// Client's billing address (line 2)
        /// </summary>
        [JsonPropertyName("billingAddressLine2")]
        public string? BillingAddressLine2 { get; set; }
        
        /// <summary>
        /// Client's city
        /// </summary>
        [JsonPropertyName("city")]
        public string? City { get; set; }
        
        /// <summary>
        /// Client's state/province
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; set; }
        
        /// <summary>
        /// Client's postal/ZIP code
        /// </summary>
        [JsonPropertyName("postalCode")]
        public string? PostalCode { get; set; }
        
        /// <summary>
        /// Client's primary contact email
        /// </summary>
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Client's phone number
        /// </summary>
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }
        
        /// <summary>
        /// Client's website
        /// </summary>
        [JsonPropertyName("website")]
        public string? Website { get; set; }
        
        /// <summary>
        /// Client's custom domain for accessing their application
        /// </summary>
        [JsonPropertyName("domainUrl")]
        public string? DomainUrl { get; set; }
        
        /// <summary>
        /// Client's database name
        /// </summary>
        [JsonPropertyName("databaseName")]
        public string? DatabaseName { get; set; }
        
        /// <summary>
        /// Admin user's first name
        /// </summary>
        [Required]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        
        /// <summary>
        /// Admin user's last name
        /// </summary>
        [Required]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        
        /// <summary>
        /// Admin user's password
        /// </summary>
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// Billing cycle (Monthly or Annual)
        /// </summary>
        [JsonPropertyName("billingCycle")]
        public BillingCycle BillingCycle { get; set; } = BillingCycle.Monthly;
        
        /// <summary>
        /// User's preferred language
        /// </summary>
        [JsonPropertyName("preferredLanguage")]
        public string? PreferredLanguage { get; set; }
        
        /// <summary>
        /// User's preferred theme
        /// </summary>
        [JsonPropertyName("preferredTheme")]
        public string? PreferredTheme { get; set; }
    }
    
    /// <summary>
    /// Response model for client registration
    /// </summary>
    public class ClientRegistrationResponse
    {
        /// <summary>
        /// Client ID
        /// </summary>
        [JsonPropertyName("clientId")]
        public Guid ClientId { get; set; }
        
        /// <summary>
        /// Company name
        /// </summary>
        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; } = string.Empty;
        
        /// <summary>
        /// Email address
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Admin user ID
        /// </summary>
        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Plan name
        /// </summary>
        [JsonPropertyName("planName")]
        public string PlanName { get; set; } = string.Empty;
        
        /// <summary>
        /// Subscription end date
        /// </summary>
        [JsonPropertyName("subscriptionEndDate")]
        public DateTime SubscriptionEndDate { get; set; }
    }
    
    /// <summary>
    /// Response model for client information
    /// </summary>
    public class ClientResponse
    {
        /// <summary>
        /// Client ID
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Company name
        /// </summary>
        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; } = string.Empty;
        
        /// <summary>
        /// VAT/tax number
        /// </summary>
        [JsonPropertyName("vatNumber")]
        public string VatNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// Country code
        /// </summary>
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Email address
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Phone number
        /// </summary>
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;
        
        /// <summary>
        /// Website
        /// </summary>
        [JsonPropertyName("website")]
        public string Website { get; set; } = string.Empty;
        
        /// <summary>
        /// Client status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// Domain URL
        /// </summary>
        [JsonPropertyName("domainUrl")]
        public string DomainUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Database name
        /// </summary>
        [JsonPropertyName("databaseName")]
        public string DatabaseName { get; set; } = string.Empty;
        
        /// <summary>
        /// Creation date
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Billing address information
        /// </summary>
        [JsonPropertyName("billingInfo")]
        public BillingAddressInfo BillingInfo { get; set; } = new BillingAddressInfo();
        
        /// <summary>
        /// Active plan information
        /// </summary>
        [JsonPropertyName("plan")]
        public ClientPlanInfo? Plan { get; set; }
        
        /// <summary>
        /// Admin user information
        /// </summary>
        [JsonPropertyName("adminUser")]
        public UserInfo? AdminUser { get; set; }
    }
    
    /// <summary>
    /// Summary response model for client listing
    /// </summary>
    public class ClientSummaryResponse
    {
        /// <summary>
        /// Client ID
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Company name
        /// </summary>
        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; } = string.Empty;
        
        /// <summary>
        /// Email address
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Client status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// Creation date
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Plan name
        /// </summary>
        [JsonPropertyName("planName")]
        public string PlanName { get; set; } = string.Empty;
        
        /// <summary>
        /// Subscription end date
        /// </summary>
        [JsonPropertyName("subscriptionEndDate")]
        public DateTime? SubscriptionEndDate { get; set; }
    }
    
    /// <summary>
    /// Billing address information
    /// </summary>
    public class BillingAddressInfo
    {
        /// <summary>
        /// Address line 1
        /// </summary>
        [JsonPropertyName("addressLine1")]
        public string AddressLine1 { get; set; } = string.Empty;
        
        /// <summary>
        /// Address line 2
        /// </summary>
        [JsonPropertyName("addressLine2")]
        public string AddressLine2 { get; set; } = string.Empty;
        
        /// <summary>
        /// City
        /// </summary>
        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;
        
        /// <summary>
        /// State/province
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;
        
        /// <summary>
        /// Postal/ZIP code
        /// </summary>
        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Country code
        /// </summary>
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Client plan information
    /// </summary>
    public class ClientPlanInfo
    {
        /// <summary>
        /// Plan ID
        /// </summary>
        [JsonPropertyName("planId")]
        public Guid PlanId { get; set; }
        
        /// <summary>
        /// Plan name
        /// </summary>
        [JsonPropertyName("planName")]
        public string PlanName { get; set; } = string.Empty;
        
        /// <summary>
        /// Start date
        /// </summary>
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// End date
        /// </summary>
        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Billing cycle
        /// </summary>
        [JsonPropertyName("billingCycle")]
        public string BillingCycle { get; set; } = string.Empty;
        
        /// <summary>
        /// Price
        /// </summary>
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        /// <summary>
        /// Auto-renew flag
        /// </summary>
        [JsonPropertyName("autoRenew")]
        public bool AutoRenew { get; set; }
    }
    
    /// <summary>
    /// User information
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        /// <summary>
        /// First name
        /// </summary>
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;
        
        /// <summary>
        /// Last name
        /// </summary>
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;
        
        /// <summary>
        /// Email address
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// Phone number
        /// </summary>
        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;
    }
}