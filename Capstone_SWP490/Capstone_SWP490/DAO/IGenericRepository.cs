using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_SWP490.DAO
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        IEnumerable<T> GetAll();
        T Create(T entity);
        int Update(T entity, int key);
        int Delete(T entity);
        int Save();
        IEnumerable<T> SQLCommand(string sql, string[] param);
    }
}
