using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public gocyberx_icpcEntities getContext()
        {
            return _entities;
        }
        public GenericRepository(gocyberx_icpcEntities _context)
        {
            this._entities = _context;
            _dbset = _context.Set<T>();
        }
        public async Task<T> Create(T entity)
        {
            _dbset.Add(entity);
            await Save();
            return entity;
        }

        public async Task<int> Delete(T entity)
        {
            _dbset.Remove(entity);
            return await Save();
        }

        public async Task<int> DeleteMany(IEnumerable<T> entity)
        {
            _dbset.RemoveRange(entity);
            return await Save();
        }
        public async Task<int> Save()
        {
            try
            {
                return await _entities.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> Update(T entity, int key)
        {
            T existing = _entities.Set<T>().Find(key);
            if (existing != null)
            {
                _entities.Entry(existing).CurrentValues.SetValues(entity);
            }
            return await Save();
        }
        public IEnumerable<T> GetAll()
        {
            return _dbset.AsEnumerable<T>();
        }
        public async Task<IEnumerable<T>> SQLCommand(string sql, object[] param)
        {
            return await Task.Run(() => _dbset.SqlQuery(sql, param).AsEnumerable<T>());
        }
        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Where(predicate).AsEnumerable();
        }

        public async Task<IEnumerable<T>> CreateMany(IEnumerable<T> entites)
        {
            _dbset.AddRange(entites);
            await Save();
            return entites;
        }
    }
}