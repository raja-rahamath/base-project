using api.Models;

namespace api.Core.Interfaces
{
    /// <summary>
    /// Interface for database installation service
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// Creates a new database and user with the specified configuration
        /// </summary>
        /// <param name="request">The installation request with connection details</param>
        /// <returns>True if successful, otherwise false</returns>
        Task<bool> CreateDatabaseAndUserAsync(InstallRequest request);
        
        /// <summary>
        /// Checks if a database or user already exists
        /// </summary>
        /// <param name="request">The installation request with connection details</param>
        /// <returns>True if the database or user exists, otherwise false</returns>
        Task<bool> CheckDatabaseOrUserExistsAsync(InstallRequest request);
        
        /// <summary>
        /// Gets all supported database types
        /// </summary>
        /// <returns>A list of supported database types</returns>
        IEnumerable<string> GetSupportedDatabaseTypes();
    }
}