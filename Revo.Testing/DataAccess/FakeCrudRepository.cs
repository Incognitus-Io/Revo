﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Revo.DataAccess.Entities;

namespace Revo.Testing.DataAccess
{
    public class FakeCrudRepository : ICrudRepository
    {
        private readonly List<EntityEntry> entities = new List<EntityEntry>();
        private readonly List<IRepositoryFilter> filters = new List<IRepositoryFilter>();

        public FakeCrudRepository()
        {
        }

        public IEnumerable<IRepositoryFilter> DefaultFilters => filters;

        public T Get<T>(object[] id) where T : class
        {
            throw new NotImplementedException();
        }

        public T Get<T>(object id) where T : class
        {
            return entities.Select(x => x.Instance).OfType<T>().First(x => HasEntityId(x, id));
        }

        public Task<T> GetAsync<T>(CancellationToken cancellationToken, params object[] id) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(params object[] id) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(object id) where T : class
        {
            return Task.FromResult(entities.Select(x => x.Instance).OfType<T>().First(x => HasEntityId(x, id)));
        }

        public Task<T> GetAsync<T>(CancellationToken cancellationToken, object id) where T : class
        {
            return GetAsync<T>(id);
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return entities.Select(x => x.Instance).OfType<T>().FirstOrDefault(predicate.Compile());
        }

        public T First<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return entities.Select(x => x.Instance).OfType<T>().First(predicate.Compile());
        }

        public Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            return Task.FromResult(entities.Select(x => x.Instance).OfType<T>().FirstOrDefault(predicate.Compile()));
        }

        public Task<T> FirstAsync<T>(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            return Task.FromResult(First(predicate));
        }

        public T Find<T>(object[] id) where T : class
        {
            throw new NotImplementedException();
        }

        public T Find<T>(object id) where T : class
        {
            return entities
                .Select(x => x.Instance)
                .OfType<T>()
                .FirstOrDefault(x => HasEntityId(x, id));
        }

        public Task<T> FindAsync<T>(CancellationToken cancellationToken, params object[] id) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync<T>(params object[] id) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> FindAsync<T>(CancellationToken cancellationToken, object id) where T : class
        {
            return FindAsync<T>(id);
        }

        public Task<T> FindAsync<T>(object id) where T : class
        {
            return Task.FromResult(Find<T>(id));
        }

        public IQueryable<T> FindAll<T>() where T : class
        {
            return new LocalDbAsyncEnumerable<T>(entities
                .Where(x => (x.State & EntityState.Added) == 0 && (x.State & EntityState.Detached) == 0)
                .Select(x => x.Instance)
                .OfType<T>());
        }

        public Task<IList<T>> FindAllAsync<T>(CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            return Task.FromResult((IList<T>)entities
                .Where(x => (x.State & EntityState.Added) == 0 && (x.State & EntityState.Detached) == 0)
                .Select(x => x.Instance)
                .OfType<T>()
                .ToList());
        }

        public IEnumerable<T> FindAllWithAdded<T>() where T : class
        {
            return new LocalDbAsyncEnumerable<T>(entities
                   .Select(x => x.Instance)
                   .OfType<T>());
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return FindAll<T>().Where(predicate);
        }

        public IEnumerable<T> WhereWithAdded<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return FindAllWithAdded<T>().Where(predicate.Compile());
        }
       
        public IEnumerable<DbEntityEntry> Entries()
        {
            throw new NotImplementedException();
        }

        public DbEntityEntry Entry(object entity)
        {
            throw new NotImplementedException();
        }

        public DbEntityEntry<T> Entry<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetEntities<T>(params EntityState[] entityStates) where T : class
        {
            IEnumerable<EntityEntry> entries = entities;
            if (entityStates?.Length > 0)
            {
                entries = entries.Where(x => entityStates.Any(s => (s & x.State) == s));
            }

            return entries
                .Select(x => x.Instance)
                .OfType<T>();
        }

        public EntityState GetEntityState<T>(T entity) where T : class
        {
            return entities.FirstOrDefault(x => x.Instance == entity)
                       ?.State ?? EntityState.Detached;
        }

        public void SetEntityState<T>(T entity, EntityState state) where T : class
        {
            EntityEntry entry = entities.FirstOrDefault(x => x.Instance == entity);
            if (entry == null)
            {
                entities.Add(new EntityEntry(entity, state));
            }
            else
            {
                entry.State = state;
            }
        }

        public bool IsAttached<T>(T entity) where T : class
        {
            return entities.Any(x => x.Instance == entity && (x.State & EntityState.Detached) != 0);
        }

        public IReadRepository IncludeFilters(params IRepositoryFilter[] repositoryFilters)
        {
            throw new NotImplementedException();
        }

        public IReadRepository ExcludeFilter(params IRepositoryFilter[] repositoryFilters)
        {
            throw new NotImplementedException();
        }

        public IReadRepository ExcludeFilters<TRepositoryFilter>() where TRepositoryFilter : IRepositoryFilter
        {
            throw new NotImplementedException();
        }

        public void Attach<T>(T entity) where T : class
        {
            if (!IsAttached(entity))
            {
                entities.Add(new EntityEntry(entity, EntityState.Unchanged));
            }
        }

        public void AttachRange<T>(IEnumerable<T> entities) where T : class
        {
            foreach (T entity in entities)
            {
                Attach(entity);
            }
        }

        public void Add<T>(T entity) where T : class
        {
            if (!IsAttached(entity))
            {
                entities.Add(new EntityEntry(entity, EntityState.Added));
            }
        }

        public void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            foreach (T entity in entities)
            {
                Add(entity);
            }
        }

        public void Remove<T>(T entity) where T : class
        {
            EntityEntry entry = entities.FirstOrDefault(x => x.Instance == entity);
            if (entry != null)
            {
                entry.State = EntityState.Deleted;
            }
        }

        public void SaveChanges()
        {
            foreach (EntityEntry entry in entities.ToList())
            {
                if ((entry.State & EntityState.Added) != 0 || (entry.State & EntityState.Modified) != 0)
                {
                    entry.State = EntityState.Unchanged;
                }
                else if ((entry.State & EntityState.Deleted) != 0)
                {
                    entities.Remove(entry);
                }
            }
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SaveChanges();
            return Task.FromResult(0);
        }

        public void Dispose()
        {
        }

        private object GetEntityId(object entity)
        {
            return entity.GetType().GetProperty("Id").GetValue(entity);
        }

        private bool HasEntityId(object entity, object id)
        {
            object entityId = GetEntityId(entity);
            return entityId.Equals(id);
        }

        public class EntityEntry
        {
            public EntityEntry(object instance, EntityState state)
            {
                Instance = instance;
                State = state;
            }

            public object Instance { get; }
            public EntityState State { get; set; }
        }
    }
}
