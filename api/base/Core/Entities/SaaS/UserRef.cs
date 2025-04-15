using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Core.Entities.SaaS
{
    /// <summary>
    /// Represents a user who can manage client billing, payments, renewals, and plan changes
    /// </summary>
    [Table("cor_users_ref")]
    public class UserRef : Entity
    {
        /// <summary>
        /// Reference to the client this user belongs to
        /// </summary>
        [Required]
        public Guid ClientId { get; set; }
        
        /// <summary>
        /// Navigation property for client
        /// </summary>
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; } = null!;
        
        /// <summary>
        /// User's first name
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        /// <summary>
        /// User's last name
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        /// <summary>
        /// User's email address (used for login)
        /// </summary>
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// User's phone number
        /// </summary>
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;
        
        /// <summary>
        /// User's role (Admin, Billing, User)
        /// </summary>
        [Required]
        public UserRole Role { get; set; } = UserRole.User;
        
        /// <summary>
        /// Hashed password for user authentication
        /// </summary>
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = string.Empty;
        
        /// <summary>
        /// Salt used for password hashing
        /// </summary>
        [Required]
        [StringLength(255)]
        public string PasswordSalt { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether the user is active or not
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Last login date and time
        /// </summary>
        public DateTime? LastLoginAt { get; set; }
        
        /// <summary>
        /// Preferred language for the user (ISO code)
        /// </summary>
        [StringLength(10)]
        public string PreferredLanguage { get; set; } = "en-US";
        
        /// <summary>
        /// User's preferred theme
        /// </summary>
        [StringLength(20)]
        public string PreferredTheme { get; set; } = "light";
    }
    
    /// <summary>
    /// User role enum
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Regular user
        /// </summary>
        User = 0,
        
        /// <summary>
        /// User with billing management permissions
        /// </summary>
        Billing = 1,
        
        /// <summary>
        /// Administrator with full permissions
        /// </summary>
        Admin = 2
    }
}