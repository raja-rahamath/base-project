using api.Core.Entities;

namespace api.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface for entity operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from Entity base class</typeparam>
    public interface IRepository<T> where T : Entity
    {
        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve</param>
        /// <returns>The entity if found, otherwise null</returns>
        Task<T?> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>A collection of entities</returns>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="entity">The entity to add</param>
        Task AddAsync(T entity);
        
        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <param name="entity">The entity to update</param>
        Task UpdateAsync(T entity);
        
        /// <summary>
        /// Deletes an entity by its ID
        /// </summary>
        /// <param name="id">The ID of the entity to delete</param>
        Task DeleteAsync(Guid id);
    }
}