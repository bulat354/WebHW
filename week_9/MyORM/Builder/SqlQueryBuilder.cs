using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyORM.Builder
{
    public abstract class SqlQueryBuilder
    {
        protected SqlCommand _command = new SqlCommand();

        public abstract SqlCommand GetSqlCommand();
    }

    public abstract class SqlNonQueryBuilder : SqlQueryBuilder { }

    public abstract class SqlScalarBuilder : SqlQueryBuilder { }

    public abstract class SqlReaderBuilder : SqlQueryBuilder { }
}
