using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace api.Models
{
    /// <summary>
    /// Request model for database installation
    /// </summary>
    public class InstallRequest
    {
        /// <summary>
        /// The type of database (MySQL, Postgres, MSSQL, Oracle)
        /// </summary>
        [Required]
        [JsonPropertyName("dbType")]
        public string DbType { get; set; } = string.Empty;
        
        /// <summary>
        /// The IP address or hostname of the database server
        /// </summary>
        [Required]
        [JsonPropertyName("serviceIP")]
        public string ServiceIP { get; set; } = string.Empty;
        
        /// <summary>
        /// The port number for the database connection
        /// </summary>
        [Required]
        [JsonPropertyName("port")]
        public string Port { get; set; } = string.Empty;
        
        /// <summary>
        /// The name of the database to create
        /// </summary>
        [Required]
        [JsonPropertyName("dbName")]
        public string DbName { get; set; } = string.Empty;
        
        /// <summary>
        /// The root/admin username for the database server
        /// </summary>
        [Required]
        [JsonPropertyName("rootUser")]
        public string RootUser { get; set; } = string.Empty;
        
        /// <summary>
        /// The root/admin password for the database server
        /// </summary>
        [Required]
        [JsonPropertyName("rootPassword")]
        public string RootPassword { get; set; } = string.Empty;
        
        /// <summary>
        /// The username for the new admin user to be created
        /// </summary>
        [Required]
        [JsonPropertyName("adminUser")]
        public string AdminUser { get; set; } = string.Empty;
        
        /// <summary>
        /// The password for the new admin user to be created
        /// </summary>
        [Required]
        [JsonPropertyName("adminPassword")]
        public string AdminPassword { get; set; } = string.Empty;
    }
}