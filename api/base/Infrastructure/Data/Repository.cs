using api.Core.Entities;
using api.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Data
{
    /// <summary>
    /// Generic repository implementation for entity operations
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from Entity base class</typeparam>
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        
        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}