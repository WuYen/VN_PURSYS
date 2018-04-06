using System;
using System.Linq;
using System.Linq.Expressions;
using ERP_V2.Models;

namespace ERP_V2.Services
{
    public interface IServiceS<T, in TKey> where T : class
    {
        T Get(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        string Create(T entity);

        string Update(T entity);

        string Delete(TKey key);
    }
}
