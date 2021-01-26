using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductCatalog.Domain.Database
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> Get();
        Task<T> Get(Guid Id);
        List<T> Find(Func<T, bool> p);
        Task<T> Save(T t);
        Task<T> Update(T t);
        bool Delete(Guid Id);
    }
}
