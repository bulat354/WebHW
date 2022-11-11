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
        protected SqlCommand _command;
        protected MiniORM _orm;

        public abstract SqlCommand GetSqlCommand();

        internal SqlQueryBuilder(SqlCommand command, MiniORM orm)
        {
            _command = command;
            _orm = orm;
        }
    }

    public abstract class SqlNonQueryBuilder : SqlQueryBuilder
    {
        public virtual int Go()
        {
            return _orm.ExecuteNonQuery(this);
        }

        internal SqlNonQueryBuilder(SqlCommand command, MiniORM orm) : base(command, orm)
        {
        }
    }

    public abstract class SqlScalarBuilder : SqlQueryBuilder
    {
        public virtual object Go()
        {
            return _orm.ExecuteScalar(this);
        }

        internal SqlScalarBuilder(SqlCommand command, MiniORM orm) : base(command, orm)
        {
        }
    }

    public abstract class SqlReaderBuilder : SqlQueryBuilder
    {
        public virtual IEnumerable<T> Go<T>()
        {
            return _orm.ExecuteReader<T>(this);
        }

        internal SqlReaderBuilder(SqlCommand command, MiniORM orm) : base(command, orm)
        {
        }
    }
}
