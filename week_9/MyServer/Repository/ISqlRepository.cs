using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Repository
{
    public interface ISqlRepository<T>
    {
        bool Insert(T entity);
        bool Update(T entity);
        bool Delete(T entity);

        List<T> GetValues(ISqlSpecification<T> specification);
    }
}
