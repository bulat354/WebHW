using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServer.Repository
{
    public interface ISqlSpecification<T>
    {
        public string ToSqlClauses();
    }
}
