namespace api.Core.Entities
{
    /// <summary>
    /// Represents a database connection configuration
    /// </summary>
    public class ConnectionConfig : Entity
    {
        /// <summary>
        /// A user-friendly name for this connection
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// The type of database (MySQL, Postgres, MSSQL, Oracle)
        /// </summary>
        public string DbType { get; set; } = string.Empty;
        
        /// <summary>
        /// The IP address or hostname of the database server
        /// </summary>
        public string ServerAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// The port number for the database connection
        /// </summary>
        public string Port { get; set; } = string.Empty;
        
        /// <summary>
        /// The name of the database
        /// </summary>
        public string DatabaseName { get; set; } = string.Empty;
        
        /// <summary>
        /// The username to connect to the database
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// Flag indicating if this connection is active
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}