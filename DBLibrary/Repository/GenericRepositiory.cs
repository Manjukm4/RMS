using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary.Repository
{
   public class GenericRepositiory<TEntity> where TEntity : class  
    {
        internal RIC_DBEntities context;
        internal DbSet<TEntity> dbSet;
        public GenericRepositiory(RIC_DBEntities context)
        {
            this.context = context;// create the object of context.
            this.dbSet = context.Set<TEntity>();
        }
        // get the data from entity.
        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
                                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            // include the properties.
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();//retun list by order.
            }
            else
            {
                return query.ToList();// retuurn list.  
            }
        }
        // get data by id.
        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }
        // insert data.
        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }
        // delete data by id.
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }
        // delete the data.
        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }
        //update the value.
        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

    }
}
