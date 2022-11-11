using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyORM.Builder
{
    public class SqlSelectBuilder : SqlReaderBuilder
    {
        private bool isDistinct = false;
        private int takeCount = -1;
        private int skipCount = -1;
        private string selectors = null;
        private string source = null;
        private string searchCondition = null;
        private string orderSelectors = null;
        private bool isOrderDesc = false;

        public SqlSelectBuilder(SqlCommand command, MiniORM orm) : base(command, orm)
        {
        }

        public override SqlCommand GetSqlCommand()
        {
            var text = new StringBuilder();
            text.Append("SELECT");
            if (isDistinct) text.Append(" DISTINCT");
            if (takeCount > 0) text.Append(" TOP @takeCount");
            if (selectors != null) text.Append($" {selectors}");
            else text.Append($" *");
            if (source != null) text.Append($"\nFROM {source}");
            if (searchCondition != null) text.Append($"\nWHERE {searchCondition}");
            if (orderSelectors != null) text.Append($"\nORDER BY {orderSelectors}");
            if (isOrderDesc) text.Append(" DESC");
            if (skipCount > 0) text.Append("\nOFFSET @skipCount ROWS");

            _command.CommandText = text.ToString();

            return _command;
        }

        public SqlSelectBuilder Select<T>()
        {
            if (source != null)
                return this;

            source = EntityModel.GetName<T>();
            return this;
        }

        public SqlSelectBuilder Select<T>(params string[] selectors)
        {
            if (selectors != null)
                return this;

            this.selectors = string.Join(", ", selectors);
            return Select<T>();
        }

        public SqlSelectBuilder Distinct()
        {
            isDistinct = true;
            return this;
        }

        public SqlSelectBuilder Take(int count)
        {
            if (takeCount > 0 || count < 1)
                return this;

            takeCount = count;
            _command.Parameters.AddWithValue("@takeCount", count);
            return this;
        }

        public SqlSelectBuilder Skip(int count)
        {
            if (skipCount > 0 || count < 1)
                return this;

            skipCount = count;
            _command.Parameters.AddWithValue("@skipCount", count);
            return this;
        }

        public SqlSelectBuilder Where(string condition, params (string, object)[] parameters)
        {
            if (searchCondition != null)
                return this;

            searchCondition = condition;
            _command.Parameters.AddRange(parameters
                                            .Select(x => new SqlParameter(x.Item1, x.Item2))
                                            .ToArray());
            return this;
        }

        public SqlSelectBuilder OrderBy(params string[] selectors)
        {
            if (orderSelectors != null)
                return this;

            orderSelectors = string.Join(", ", selectors);
            return this;
        }

        public SqlSelectBuilder OrderByDescending(params string[] selectors)
        {
            if (orderSelectors != null)
                return this;

            isOrderDesc = true;
            return OrderBy(selectors);
        }
    }
}
