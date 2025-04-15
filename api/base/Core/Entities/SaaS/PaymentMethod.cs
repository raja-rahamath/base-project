using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Core.Entities.SaaS
{
    /// <summary>
    /// Represents a client's payment method (credit card, etc.)
    /// </summary>
    [Table("cor_payment_methods")]
    public class PaymentMethod : Entity
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
        /// Payment method type
        /// </summary>
        [Required]
        public PaymentMethodType Type { get; set; } = PaymentMethodType.CreditCard;
        
        /// <summary>
        /// Last 4 digits of credit card number (for display purposes)
        /// </summary>
        [StringLength(4)]
        public string Last4 { get; set; } = string.Empty;
        
        /// <summary>
        /// Card brand (Visa, Mastercard, etc.)
        /// </summary>
        [StringLength(20)]
        public string CardBrand { get; set; } = string.Empty;
        
        /// <summary>
        /// Card expiration month
        /// </summary>
        public int? ExpiryMonth { get; set; }
        
        /// <summary>
        /// Card expiration year
        /// </summary>
        public int? ExpiryYear { get; set; }
        
        /// <summary>
        /// Cardholder name
        /// </summary>
        [StringLength(100)]
        public string CardholderName { get; set; } = string.Empty;
        
        /// <summary>
        /// Token or identifier from payment processor (encrypted)
        /// </summary>
        [StringLength(500)]
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether this is the default payment method
        /// </summary>
        [Required]
        public bool IsDefault { get; set; } = false;
        
        /// <summary>
        /// Whether the payment method is active or not
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Notes about the payment method
        /// </summary>
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Payment method type enum
    /// </summary>
    public enum PaymentMethodType
    {
        /// <summary>
        /// Credit card payment method
        /// </summary>
        CreditCard = 0,
        
        /// <summary>
        /// Bank transfer payment method
        /// </summary>
        BankTransfer = 1,
        
        /// <summary>
        /// PayPal payment method
        /// </summary>
        PayPal = 2
    }
}