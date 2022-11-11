using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORM.Builder
{
    public class SqlDeleteBuilder : SqlNonQueryBuilder
    {
        private string source = null;
        private string searchCondition = null;

        public SqlDeleteBuilder(SqlCommand command, MiniORM orm) : base(command, orm)
        {
        }

        public override SqlCommand GetSqlCommand()
        {
            var text = new StringBuilder();
            text.Append("DELETE");
            if (source != null) text.Append(" FROM " + source);
            if (searchCondition != null) text.Append("\nWHERE " + searchCondition);

            _command.CommandText = text.ToString();
            return _command;
        }

        public SqlDeleteBuilder Delete<T>()
        {
            if (source != null)
                return this;

            source = EntityModel.GetName<T>();

            return this;
        }

        public SqlDeleteBuilder Where(string condition, params (string, object)[] parameters)
        {
            if (searchCondition != null)
                return this;

            searchCondition = condition;
            _command.Parameters.AddRange(parameters
                                            .Select(x => new SqlParameter(x.Item1, x.Item2))
                                            .ToArray());
            return this;
        }
    }
}
