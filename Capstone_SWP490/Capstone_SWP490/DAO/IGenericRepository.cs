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
        Task<T> Create(T entity);
        Task<IEnumerable<T>> CreateMany(IEnumerable<T> entites);
        Task<int> Update(T entity, int key);
        Task<int> Delete(T entity);
        Task<int> Save();
        Task<IEnumerable<T>> SQLCommand(string sql, string[] param);

    }
}
