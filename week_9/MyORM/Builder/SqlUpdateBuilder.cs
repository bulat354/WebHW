using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORM.Builder
{
    public class SqlUpdateBuilder : SqlNonQueryBuilder
    {
        private string set = null;
        private string source = null;
        private string searchCondition = null;

        public override SqlCommand GetSqlCommand()
        {
            var text = new StringBuilder();
            text.Append("UPDATE");
            if (source != null) text.Append(" " + source);
            if (set != null) text.Append("\nVALUES " + set);
            if (searchCondition != null) text.Append("\nWHERE " + searchCondition);

            _command.CommandText = text.ToString();
            return _command;
        }

        public SqlUpdateBuilder Update<T>(T obj)
        {
            if (source != null)
                return this;

            source = EntityModel.GetName<T>();

            var properties = EntityModel.GetEditableProperties<T>().ToArray();
            set = string.Join(", ", properties.Select(x => $"{EntityModel.GetColumnName(x)} = @{EntityModel.GetColumnName(x)}"));

            _command.Parameters.AddRange(properties
                .Select(x => new SqlParameter("@" + EntityModel.GetColumnName(x), x.GetValue(obj)))
                .ToArray());

            return this;
        }

        public SqlUpdateBuilder Where(string condition, params SqlParameter[] parameters)
        {
            if (searchCondition != null)
                return this;

            searchCondition = condition;
            _command.Parameters.AddRange(parameters);
            return this;
        }
    }
}
