// Copyright © Christopher Dorst. All rights reserved.
// Licensed under the GNU General Public License, Version 3.0. See the LICENSE document in the repository root for license information.

using DevOps.Code.Entities.Interfaces.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DevOps.Code.DataAccess.Services.DataRepository
{
    /// <summary>Represents a generic data-access repository</summary>
    public class Repository<TDbContext, TEntity, TKey> : IRepository<TDbContext, TEntity, TKey>
        where TDbContext : DbContext
        where TEntity : class, IEntity<TKey>
        where TKey : struct
    {
        /// <summary>Logger</summary>
        protected readonly ILogger<Repository<TDbContext, TEntity, TKey>> _logger;

        /// <summary>Represents the EntityFrameworkCore database context</summary>
        private readonly TDbContext _context;

        /// <summary>Represents the EntityFrameworkCore DbSet (database table)</summary>
        private readonly DbSet<TEntity> _set;

        /// <summary>Constructs a repository instance using the given database context</summary>
        public Repository(TDbContext context, ILogger<Repository<TDbContext, TEntity, TKey>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _set = _context.Set<TEntity>();
        }

        /// <summary>Adds the entity to the data repository</summary>
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null) return null;
            _logger.LogInformation("Adding entity to db context");
            _set.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>Finds an entity with the given key</summary>
        public virtual async Task<TEntity> FindAsync(TKey key)
        {
            _logger.LogInformation("Finding entity");
            return await _set.FindAsync(key);
        }

        /// <summary>Removes the entity from the data repository</summary>
        public virtual async Task RemoveAsync(TKey key)
        {
            _logger.LogInformation("Removing entity");
            var entity = await FindAsync(key);
            if (entity == null) return;
            _set.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>Replaces an entity in the data repository</summary>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null) return null;
            _logger.LogInformation("Updating entity");
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
