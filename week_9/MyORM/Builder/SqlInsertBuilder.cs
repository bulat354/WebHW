using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORM.Builder
{
    public class SqlInsertBuilder : SqlNonQueryBuilder
    {
        private List<string> values = null;
        private string source = null;

        public SqlInsertBuilder(SqlCommand command, MiniORM orm) : base(command, orm)
        {
        }

        public override SqlCommand GetSqlCommand()
        {
            var text = new StringBuilder();
            text.Append("INSERT");
            if (source != null) text.Append($" INTO {source} VALUES\n");
            if (values != null) text.Append(string.Join("\n", values));

            _command.CommandText = text.ToString();
            return _command;
        }

        public SqlInsertBuilder Insert<T>(params T[] objs)
        {
            if (source != null)
                return this;

            source = EntityModel.GetName<T>();

            var properties = EntityModel.GetEditableProperties<T>().ToArray();
            var names = properties.Select(x => EntityModel.GetColumnName(x)).ToArray();

            for (int i = 0; i < objs.Length; i++)
            {
                var paramNames = names.Select(x => $"@{x}.{i}").ToArray();
                values.Add($"({string.Join(", ", paramNames)})");
                _command.Parameters.AddRange(properties
                    .Zip(paramNames, (p, n) => new SqlParameter(n, p.GetValue(objs[i])))
                    .ToArray());
            }

            return this;
        }
    }
}
