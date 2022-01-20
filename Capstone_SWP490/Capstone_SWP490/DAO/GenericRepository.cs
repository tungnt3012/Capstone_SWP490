using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Capstone_SWP490.DAO
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbset;
        private readonly gocyberx_icpcEntities _entities;

        public GenericRepository()
        {
            this._entities = new gocyberx_icpcEntities();
            _dbset = _entities.Set<T>();
        }
        public GenericRepository(gocyberx_icpcEntities _context)
        {
            this._entities = _context;
            _dbset = _context.Set<T>();
        }

        public T Create(T entity)
        {
            _dbset.Add(entity);
            Save();
            return entity;
        }

        public int Delete(T entity)
        {
            _dbset.Remove(entity);
            return Save();
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Where(predicate).AsEnumerable();
        }

        public IEnumerable<T> GetAll()
        {
           return _dbset.AsEnumerable<T>();
        }

        public int Save()
        {
            try
            {
                return _entities.SaveChanges();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public IEnumerable<T> SQLCommand(string sql, string[] param)
        {
            return _dbset.SqlQuery(sql, param).AsEnumerable<T>();
        }

        public int Update(T entity, int key)
        {
            T existing = _entities.Set<T>().Find(key);
            if (existing != null)
            {
                _entities.Entry(existing).CurrentValues.SetValues(entity);
            }
            return Save();
        }
    }
}