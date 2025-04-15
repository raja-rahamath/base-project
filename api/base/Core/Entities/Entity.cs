namespace api.Core.Entities
{
    /// <summary>
    /// Base entity that all domain entities inherit from
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Unique identifier for the entity
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        
        /// <summary>
        /// Date when the entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}