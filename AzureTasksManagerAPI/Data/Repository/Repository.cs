using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AzureTasksManagerAPI.DataAccess.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AzureTasksManagerAPI.DataAccess.Repository
{
    public class Repository<T>
        : IRepository<T>
        where T : class, new()
    {
        protected readonly ApplicationDbContext db;

        protected internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            this.db = db;
            this.dbSet = this.db.Set<T>();
        }

        public void Add(T entity)
        {
            this.dbSet.Add(entity);
        }

        public ValueTask<T> Get(int? id)
        {
            return this.dbSet.FindAsync(id);
        }

        public Task<List<T>> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            IQueryable<T> query = this.dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).ToListAsync();
            }

            return query.ToListAsync();
        }

        public void Remove(int? id)
        {
            T entity = this.dbSet.Find(id);
            Remove(entity);
        }

        public void Remove(T entity)
        {
            this.dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            this.dbSet.RemoveRange(entities);
        }

        public virtual void Update(T entity)
        {
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            T newEntity = (T)Activator.CreateInstance(typeof(T));
            foreach (PropertyInfo property in propertyInfos)
            {
                property.SetValue(newEntity, property.GetValue(entity));
            }

            this.dbSet.Update(newEntity);
        }
    }
}